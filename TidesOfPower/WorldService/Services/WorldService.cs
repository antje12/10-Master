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

    private KafkaAdministrator _admin;
    private KafkaProducer<LocalState_M> _producerLS;
    private KafkaProducer<Projectile_M> _producerP;
    private KafkaProducer<Ai_M> _producerA;
    private KafkaConsumer<World_M> _consumer;

    private MongoDbBroker _mongoBroker;
    private RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public WorldService()
    {
        Console.WriteLine("WorldService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producerLS = new KafkaProducer<LocalState_M>(config);
        _producerP = new KafkaProducer<Projectile_M>(config);
        _producerA = new KafkaProducer<Ai_M>(config);
        _consumer = new KafkaConsumer<World_M>(config);
        _mongoBroker = new MongoDbBroker(localTest);
        _redisBroker = new RedisBroker(localTest);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("WorldService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<World_M>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("WorldService stopped");
    }

    private void ProcessMessage(string key, World_M value)
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
        }
    }

    private void MovePlayer(string key, World_M value)
    {
        var agent = new Agent_M
        {
            Id = value.EntityId,
            Location = value.Location
        };
        _redisBroker.UpsertAgentLocation(new Player(
            "",
            0,
            Guid.Parse(agent.Id),
            new Coordinates(agent.Location.X, agent.Location.Y),
            100,
            100));

        var entities = _redisBroker.GetEntities(agent.Location.X, agent.Location.Y);
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
                }
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
        _producerLS.Produce($"{_outputTopicLS}_{value.EntityId}", key, msgOut);
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
            _producerLS.Produce($"{_outputTopicLS}_{player.Id}", player.Id.ToString(), msgOut);
        }
    }

    private void MoveAi(string key, World_M value)
    {
        if (_redisBroker.Get(Guid.Parse(value.EntityId)) == null)
            return;

        var msgOut = new Ai_M
        {
            Id = value.EntityId,
            Location = value.Location,
            LastUpdate = value.LastUpdate
        };
        _redisBroker.UpsertAgentLocation(new Enemy(
            Guid.Parse(msgOut.Id),
            new Coordinates(msgOut.Location.X, msgOut.Location.Y),
            100,
            100));
        _producerA.Produce(_outputTopicA, msgOut.Id, msgOut);

        var agent = new Agent_M
        {
            Id = value.EntityId,
            Location = value.Location
        };

        var players = _redisBroker
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
            _producerP.Produce(_outputTopicP, msgOut.Id, msgOut);
            sync = Sync.Delta;
        }

        var players = _redisBroker
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
        _redisBroker.UpsertAgentLocation(new Enemy(
            Guid.Parse(msgOut.Id),
            new Coordinates(msgOut.Location.X, msgOut.Location.Y),
            100,
            100));
        _producerA.Produce(_outputTopicA, msgOut.Id, msgOut);

        var agent = new Agent_M
        {
            Id = msgOut.Id,
            Location = msgOut.Location
        };

        var players = _redisBroker
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
        _producerP.Produce(_outputTopicP, msgOut.Id, msgOut);

        var players = _redisBroker
            .GetEntities(msgOut.Location.X, msgOut.Location.Y)
            .OfType<Player>().ToList();
        DeltaSync(players, new List<Agent_M>(), new List<Projectile_M>(){msgOut}, new List<Treasure_M>(), Sync.Delta);
    }

    private void DamageAgent(string key, World_M value)
    {
        var player = _redisBroker.Get(Guid.Parse(value.EntityId));
        if (player is Player t && t.Score > 0)
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

        var players = _redisBroker
            .GetEntities(value.Location.X, value.Location.Y)
            .OfType<Player>().ToList();
        _redisBroker.DeleteEntity(Guid.Parse(msgOut.Id));
        DeltaSync(players, new List<Agent_M>(){msgOut}, new List<Projectile_M>(), new List<Treasure_M>(), Sync.Delete);
    }
    
    private void SpawnTreasure(string key, World_M value)
    {
        Console.WriteLine($"SpawnTreasure: {value.Value}");
        var treasure = new Treasure(value.Value, Guid.NewGuid(), new Coordinates(value.Location.X, value.Location.Y));
        _redisBroker.Insert(treasure);

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

        var players = _redisBroker
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

        var players = _redisBroker
            .GetEntities(value.Location.X, value.Location.Y)
            .OfType<Player>().ToList();
        _redisBroker.DeleteEntity(Guid.Parse(msgOut.Id));
        DeltaSync(players, new List<Agent_M>(), new List<Projectile_M>(), new List<Treasure_M>(){msgOut}, Sync.Delete);
    }
}