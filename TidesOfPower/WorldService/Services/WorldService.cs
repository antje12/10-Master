using System.Diagnostics;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using WorldService.Interfaces;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;
using AiAgent = ClassLibrary.Messages.Protobuf.AiAgent;
using Avatar = ClassLibrary.Messages.Protobuf.Avatar;
using Coordinates = ClassLibrary.Messages.Protobuf.Coordinates;
using Projectile = ClassLibrary.Messages.Protobuf.Projectile;

namespace WorldService.Services;

public class WorldService : BackgroundService, IConsumerService
{
    private string _groupId = "world-group";
    private KafkaTopic _inputTopic = KafkaTopic.World;
    private KafkaTopic _outputTopicLS = KafkaTopic.LocalState;
    private KafkaTopic _outputTopicP = KafkaTopic.Projectile;
    private KafkaTopic _outputTopicA = KafkaTopic.Ai;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<LocalState> _producerLS;
    private ProtoKafkaProducer<Projectile> _producerP;
    private ProtoKafkaProducer<AiAgent> _producerA;
    private ProtoKafkaConsumer<WorldChange> _consumer;

    private MongoDbBroker _mongoBroker;
    private RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public WorldService()
    {
        Console.WriteLine("WorldService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producerLS = new ProtoKafkaProducer<LocalState>(config);
        _producerP = new ProtoKafkaProducer<Projectile>(config);
        _producerA = new ProtoKafkaProducer<AiAgent>(config);
        _consumer = new ProtoKafkaConsumer<WorldChange>(config);
        _mongoBroker = new MongoDbBroker(localTest);
        _redisBroker = new RedisBroker(localTest);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("WorldService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<WorldChange>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("WorldService stopped");
    }

    private void ProcessMessage(string key, WorldChange value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(key, value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(string key, WorldChange value)
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

    private void MovePlayer(string key, WorldChange value)
    {
        var player = new Avatar()
        {
            Id = value.EntityId,
            Location = value.Location
        };
        _redisBroker.UpsertAvatarLocation(new ClassLibrary.Classes.Domain.Player()
        {
            Id = Guid.Parse(player.Id),
            Location = new ClassLibrary.Classes.Data.Coordinates()
            {
                X = player.Location.X,
                Y = player.Location.Y
            }
        });

        var entities = _redisBroker.GetEntities(player.Location.X, player.Location.Y);
        FullSync(key, value, entities);

        var players = entities
            .OfType<ClassLibrary.Classes.Domain.Player>()
            .Where(x => x.Id.ToString() != player.Id).ToList();
        DeltaSync(players, [player], [], Sync.Delta);
    }

    private void FullSync(string key, WorldChange value, List<Entity> entities)
    {
        var msgOut = new LocalState()
        {
            AgentId = value.EntityId,
            Sync = Sync.Full,
            EventId = value.EventId
        };
        var avatars = entities.OfType<ClassLibrary.Classes.Domain.Avatar>()
            .Select(a => new Avatar()
            {
                Id = a.Id.ToString(),
                Location = new Coordinates()
                {
                    X = a.Location.X,
                    Y = a.Location.Y,
                }
            }).ToList();
        var projectiles = entities.OfType<ClassLibrary.Classes.Domain.Projectile>()
            .Select(p => new Projectile()
            {
                Id = p.Id.ToString(),
                Location = new Coordinates()
                {
                    X = p.Location.X,
                    Y = p.Location.Y,
                }
            }).ToList();
        msgOut.Avatars.AddRange(avatars);
        msgOut.Projectiles.AddRange(projectiles);
        _producerLS.Produce($"{_outputTopicLS}_{msgOut.AgentId}", key, msgOut);
    }

    private void DeltaSync(
        List<Player> players,
        List<Avatar> avatars,
        List<Projectile> projectiles,
        Sync sync)
    {
        foreach (var player in players)
        {
            var msgOut = new LocalState()
            {
                AgentId = player.Id.ToString(),
                Sync = sync
            };
            msgOut.Avatars.AddRange(avatars);
            msgOut.Projectiles.AddRange(projectiles);
            _producerLS.Produce($"{_outputTopicLS}_{msgOut.AgentId}", msgOut.AgentId, msgOut);
        }
    }

    private void MoveAi(string key, WorldChange value)
    {
        var msgOut = new AiAgent()
        {
            Id = value.EntityId,
            Location = value.Location,
            LastUpdate = value.LastUpdate
        };
        _redisBroker.UpsertAvatarLocation(new ClassLibrary.Classes.Domain.AiAgent()
        {
            Id = Guid.Parse(msgOut.Id),
            Location = new ClassLibrary.Classes.Data.Coordinates()
            {
                X = msgOut.Location.X,
                Y = msgOut.Location.Y
            }
        });
        _producerA.Produce(_outputTopicA, msgOut.Id, msgOut);

        var avatar = new Avatar()
        {
            Id = value.EntityId,
            Location = value.Location
        };
        
        var players = _redisBroker
            .GetEntities(msgOut.Location.X, msgOut.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [avatar], [], Sync.Delta);
    }

    private void MoveBullet(string key, WorldChange value)
    {
        var msgOut = new Projectile()
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
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [], [msgOut], sync);
    }

    private void SpawnAi(string key, WorldChange value)
    {
        var msgOut = new AiAgent()
        {
            Id = Guid.NewGuid().ToString(),
            Location = new Coordinates() {X = value.Location.X, Y = value.Location.Y},
            LastUpdate = DateTime.UtcNow.Ticks,
        };
        _producerA.Produce(_outputTopicA, msgOut.Id, msgOut);
        
        var avatar = new Avatar()
        {
            Id = msgOut.Id,
            Location = msgOut.Location
        };
        
        var players = _redisBroker
            .GetEntities(msgOut.Location.X, msgOut.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [avatar], [], Sync.Delta);
    }

    private void SpawnBullet(string key, WorldChange value)
    {
        var msgOut = new Projectile()
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
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [], [msgOut], Sync.Delta);
    }

    private void DamageAgent(string key, WorldChange value)
    {
        var player = new Avatar()
        {
            Id = value.EntityId
        };
        _redisBroker.Delete(new ClassLibrary.Classes.Domain.Avatar()
        {
            Id = Guid.Parse(player.Id)
        });

        var players = _redisBroker
            .GetEntities(value.Location.X, value.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [player], [], Sync.Delete);
    }
}