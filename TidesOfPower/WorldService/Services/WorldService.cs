using System.Diagnostics;
using ClassLibrary.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.MongoDB;
using ClassLibrary.Redis;
using WorldService.Interfaces;

namespace WorldService.Services;

public class WorldService : BackgroundService, IConsumerService
{
    private string _groupId = "world-group";
    private KafkaTopic _inputTopic = KafkaTopic.World;
    private KafkaTopic _outputTopicLS = KafkaTopic.LocalState;
    private KafkaTopic _outputTopicP = KafkaTopic.Projectile;
    private KafkaTopic _outputTopicA = KafkaTopic.Ai;

    internal IAdministrator Admin;
    internal IProtoProducer<LocalState_M> ProducerLS;
    internal IProtoProducer<Projectile_M> ProducerP;
    internal IProtoProducer<Ai_M> ProducerA;
    internal IProtoConsumer<World_M> Consumer;

    internal MongoDbBroker MongoBroker;
    internal RedisBroker RedisBroker;
    
    private Dictionary<string, DateTime> ClientUpdates = new();
    
    private CancellationTokenSource _cts = new();
    public bool IsRunning { get; private set; }
    private bool localTest = false;

    public WorldService()
    {
        Console.WriteLine("WorldService created");
        var config = new KafkaConfig(_groupId, localTest);
        Admin = new KafkaAdministrator(config);
        ProducerLS = new KafkaProducer<LocalState_M>(config);
        ProducerP = new KafkaProducer<Projectile_M>(config);
        ProducerA = new KafkaProducer<Ai_M>(config);
        Consumer = new KafkaConsumer<World_M>(config);
        MongoBroker = new MongoDbBroker();
        RedisBroker = new RedisBroker();
    }

    public void StopService()
    {
        _cts.Cancel();
    }

    internal async Task ExecuteAsync()
    {
        await ExecuteAsync(_cts.Token);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        RedisBroker.Connect(localTest);
        MongoBroker.Connect(localTest);
        Console.WriteLine("WorldService started");
        await Admin.CreateTopic(_inputTopic);
        IProtoConsumer<World_M>.ProcessMessage action = ProcessMessage;
        var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);
        await Consumer.Consume(_inputTopic, action, linkedSource.Token);
        IsRunning = false;
        Console.WriteLine("WorldService stopped");
    }

    internal void ProcessMessage(string key, World_M value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(key, value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(string key, World_M value)
    {
        if (!string.IsNullOrEmpty(value.EventId))
        {
            string timestampWithMs = DateTime.UtcNow.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Got {value.EventId} at {timestampWithMs}");
        }
        
        var oldKeys = ClientUpdates.Where(x => x.Value < DateTime.UtcNow)
            .Select(x => x.Key);
        foreach (var oldKey in oldKeys)
        {
            ClientUpdates.Remove(oldKey);
        }
        
        switch (value.Change)
        {
            case Change.MovePlayer:
                MovePlayer(key, value);
                break;
            case Change.MoveAi:
                MoveAi(key, value);
                break;
            case Change.MoveBullet:
                MoveBullet(key, value);
                break;
            case Change.SpawnAi:
                SpawnAi(key, value);
                break;
            case Change.SpawnBullet:
                SpawnBullet(key, value);
                break;
            case Change.DamageAgent:
                DamageAgent(key, value);
                break;
            case Change.CollectTreasure:
                CollectTreasure(key, value);
                break;
        }
    }

    private void MovePlayer(string key, World_M value)
    {
        var red = RedisBroker.Get(Guid.Parse(value.EntityId));
        if (red == null)
        {
            var mongo = MongoBroker.GetPlayer(Guid.Parse(value.EntityId));
            if (mongo == null)
            {
                mongo = new Player(
                    "Player",
                    0,
                    Guid.Parse(value.EntityId),
                    new Coordinates(value.Location.X, value.Location.Y),
                    100,
                    100);
                MongoBroker.Insert(mongo);
                ClientUpdates[key] = DateTime.UtcNow.AddMinutes(1);
            }
            else
            {
                value.Location = new Coordinates_M(){X=mongo.Location.X,Y=mongo.Location.Y};
            }
            red = mongo;
        }
        
        var xDiff = Math.Abs(value.Location.X - red.Location.X);
        var yDiff = Math.Abs(value.Location.Y - red.Location.Y);
        if (xDiff > 50 || yDiff > 50)
            value.Location = new Coordinates_M(){X=red.Location.X,Y=red.Location.Y};
        
        var agent = new Agent_M
        {
            Id = value.EntityId,
            Location = value.Location,
            Name = red is Player p ? p.Name : "Player",
            WalkingSpeed = red is Player p2 ? p2.WalkingSpeed : 0,
            LifePool = red is Player p3 ? p3.LifePool : 0,
            Score = red is Player p4 ? p4.Score : 0
        };
        agent.Score += value.Value;
        var data = new Player(
            agent.Name,
            agent.Score,
            Guid.Parse(agent.Id),
            new Coordinates(agent.Location.X, agent.Location.Y),
            100,
            100);
        RedisBroker.UpsertAgentLocation(data);

        if (!ClientUpdates.ContainsKey(key))
        {
            MongoBroker.UpdatePlayer(data);
            ClientUpdates[key] = DateTime.UtcNow.AddMinutes(1);
        }
        
        var entities = RedisBroker.GetEntities(agent.Location.X, agent.Location.Y);
        FullSync(key, value, entities);

        var otherPlayers = entities
            .OfType<Player>()
            .Where(x => x.Id.ToString() != agent.Id).ToList();
        DeltaSync(otherPlayers, new List<Agent_M>(){agent}, new List<Projectile_M>(), new List<Treasure_M>(), Sync.Delta);
    }

    private void FullSync(string key, World_M value, List<Entity> entities)
    {
        var msgOut = new LocalState_M
        {
            Sync = Sync.Full,
            EventId = value.EventId
        };
        var agents = entities.OfType<Agent>()
            .Select(a => new Agent_M
            {
                Id = a.Id.ToString(),
                Location = new Coordinates_M
                {
                    X = a.Location.X,
                    Y = a.Location.Y,
                },
                Name = a is Player p ? p.Name : "AI",
                WalkingSpeed = a is Player p2 ? p2.WalkingSpeed : 0,
                LifePool = a is Player p3 ? p3.LifePool : 0,
                Score = a is Player p4 ? p4.Score : 0
            }).ToList();
        var projectiles = entities.OfType<Projectile>()
            .Select(p => new Projectile_M
            {
                Id = p.Id.ToString(),
                Location = new Coordinates_M
                {
                    X = p.Location.X,
                    Y = p.Location.Y,
                }
            }).ToList();
        var treasures = entities.OfType<Treasure>()
            .Select(t => new Treasure_M()
            {
                Id = t.Id.ToString(),
                Location = new Coordinates_M
                {
                    X = t.Location.X,
                    Y = t.Location.Y,
                },
                Value = t.Value
            }).ToList();
        msgOut.Agents.AddRange(agents);
        msgOut.Projectiles.AddRange(projectiles);
        msgOut.Treasures.AddRange(treasures);
        ProducerLS.Produce($"{_outputTopicLS}_{value.EntityId}", key, msgOut);
        
        if (!string.IsNullOrEmpty(value.EventId))
        {
            string timestampWithMs = DateTime.UtcNow.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Sent {value.EventId} at {timestampWithMs}");
        }
    }

    private void DeltaSync(
        List<Player> players,
        List<Agent_M> agents,
        List<Projectile_M> projectiles,
        List<Treasure_M> treasures,
        Sync sync)
    {
        foreach (var player in players)
        {
            var msgOut = new LocalState_M
            {
                Sync = sync
            };
            msgOut.Agents.AddRange(agents);
            msgOut.Projectiles.AddRange(projectiles);
            msgOut.Treasures.AddRange(treasures);
            ProducerLS.Produce($"{_outputTopicLS}_{player.Id}", player.Id.ToString(), msgOut);
        }
    }

    private void MoveAi(string key, World_M value)
    {
        var db = RedisBroker.Get(Guid.Parse(value.EntityId));
        if (db == null)
            return;

        var msgOut = new Ai_M
        {
            Id = value.EntityId,
            Location = value.Location,
            LastUpdate = value.LastUpdate
        };
        RedisBroker.UpsertAgentLocation(new Enemy(
            Guid.Parse(msgOut.Id),
            new Coordinates(msgOut.Location.X, msgOut.Location.Y),
            100,
            100));
        ProducerA.Produce(_outputTopicA, msgOut.Id, msgOut);

        var agent = new Agent_M
        {
            Id = value.EntityId,
            Location = value.Location,
            Name = "AI",
            WalkingSpeed = 100,
            LifePool = db is Enemy e ? e.LifePool : 0,
            Score = 0
        };

        var players = RedisBroker
            .GetEntities(msgOut.Location.X, msgOut.Location.Y)
            .OfType<Player>().ToList();
        DeltaSync(players, new List<Agent_M>(){agent}, new List<Projectile_M>(), new List<Treasure_M>(), Sync.Delta);
    }

    private void MoveBullet(string key, World_M value)
    {
        var msgOut = new Projectile_M
        {
            Id = value.EntityId,
            Location = value.Location,
            Direction = value.Direction,
            LastUpdate = value.LastUpdate,
            TTL = value.TTL
        };

        Sync sync;
        if (msgOut.TTL <= 0)
        {
            sync = Sync.Delete;
        }
        else
        {
            ProducerP.Produce(_outputTopicP, msgOut.Id, msgOut);
            sync = Sync.Delta;
        }

        var players = RedisBroker
            .GetEntities(msgOut.Location.X, msgOut.Location.Y)
            .OfType<Player>().ToList();
        DeltaSync(players, new List<Agent_M>(), new List<Projectile_M>(){msgOut}, new List<Treasure_M>(), sync);
    }

    private void SpawnAi(string key, World_M value)
    {
        var msgOut = new Ai_M
        {
            Id = Guid.NewGuid().ToString(),
            Location = new Coordinates_M {X = value.Location.X, Y = value.Location.Y},
            LastUpdate = DateTime.UtcNow.Ticks,
        };
        RedisBroker.UpsertAgentLocation(new Enemy(
            Guid.Parse(msgOut.Id),
            new Coordinates(msgOut.Location.X, msgOut.Location.Y),
            100,
            100));
        ProducerA.Produce(_outputTopicA, msgOut.Id, msgOut);

        var agent = new Agent_M
        {
            Id = msgOut.Id,
            Location = msgOut.Location,
            Name = "AI",
            WalkingSpeed = 100,
            LifePool = 100,
            Score = 0
        };

        var players = RedisBroker
            .GetEntities(msgOut.Location.X, msgOut.Location.Y)
            .OfType<Player>().ToList();
        DeltaSync(players, new List<Agent_M>(){agent}, new List<Projectile_M>(), new List<Treasure_M>(), Sync.Delta);
    }

    private void SpawnBullet(string key, World_M value)
    {
        var msgOut = new Projectile_M
        {
            Id = value.EntityId,
            Location = value.Location,
            Direction = value.Direction,
            LastUpdate = DateTime.UtcNow.Ticks,
            TTL = 100
        };
        ProducerP.Produce(_outputTopicP, msgOut.Id, msgOut);

        var players = RedisBroker
            .GetEntities(msgOut.Location.X, msgOut.Location.Y)
            .OfType<Player>().ToList();
        DeltaSync(players, new List<Agent_M>(), new List<Projectile_M>(){msgOut}, new List<Treasure_M>(), Sync.Delta);
    }

    private void DamageAgent(string key, World_M value)
    {
        var db = RedisBroker.Get(Guid.Parse(value.EntityId));
        if (db is Player t)
        {
            value.Value = t.Score;
        }
        else
        {
            value.Value = 10;
        }
        SpawnTreasure(key, value);
        
        var msgOut = new Agent_M
        {
            Id = value.EntityId
        };

        var players = RedisBroker
            .GetEntities(value.Location.X, value.Location.Y)
            .OfType<Player>().ToList();
        RedisBroker.DeleteEntity(Guid.Parse(msgOut.Id));
        MongoBroker.UpdatePlayer(new Player("Player", 0, Guid.Parse(msgOut.Id), 
            new Coordinates(0,0), 100, 100));
        ClientUpdates.Remove(key);
        DeltaSync(players, new List<Agent_M>(){msgOut}, new List<Projectile_M>(), new List<Treasure_M>(), Sync.Delete);
    }
    
    private void SpawnTreasure(string key, World_M value)
    {
        Console.WriteLine($"SpawnTreasure: {value.Value}");
        var treasure = new Treasure(value.Value, Guid.NewGuid(), new Coordinates(value.Location.X, value.Location.Y));
        RedisBroker.Insert(treasure);

        var msgOut = new Treasure_M()
        {
            Id = treasure.Id.ToString(),
            Location = new Coordinates_M
            {
                X = treasure.Location.X,
                Y = treasure.Location.Y,
            },
            Value = treasure.Value
        };

        var players = RedisBroker
            .GetEntities(msgOut.Location.X, msgOut.Location.Y)
            .OfType<Player>().ToList();
        DeltaSync(players, new List<Agent_M>(), new List<Projectile_M>(), new List<Treasure_M>(){msgOut}, Sync.Delta);
    }
    
    private void CollectTreasure(string key, World_M value)
    {
        var msgOut = new Treasure_M()
        {
            Id = value.EntityId
        };

        var players = RedisBroker
            .GetEntities(value.Location.X, value.Location.Y)
            .OfType<Player>().ToList();
        RedisBroker.DeleteEntity(Guid.Parse(msgOut.Id));
        DeltaSync(players, new List<Agent_M>(), new List<Projectile_M>(), new List<Treasure_M>(){msgOut}, Sync.Delete);
    }
}