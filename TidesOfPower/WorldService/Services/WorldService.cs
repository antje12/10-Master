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
using ChangeType = ClassLibrary.Messages.Protobuf.ChangeType;
using Coordinates = ClassLibrary.Messages.Protobuf.Coordinates;
using Projectile = ClassLibrary.Messages.Protobuf.Projectile;
using SyncType = ClassLibrary.Messages.Protobuf.SyncType;

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
            case ChangeType.MovePlayer:
                MovePlayer(key, value);
                break;
            case ChangeType.MoveAi:
                MoveAi(key, value);
                break;
            case ChangeType.MoveBullet:
                MoveBullet(key, value);
                break;
            case ChangeType.SpawnAi:
                SpawnAi(key, value);
                break;
            case ChangeType.SpawnBullet:
                SpawnBullet(key, value);
                break;
            case ChangeType.DamageAgent:
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
        var output = new LocalState()
        {
            PlayerId = player.Id,
            Sync = SyncType.Full,
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
        output.Avatars.AddRange(avatars);
        output.Projectiles.AddRange(projectiles);
        _producerLS.Produce($"{_outputTopicLS}_{output.PlayerId}", key, output);

        var players = entities
            .OfType<ClassLibrary.Classes.Domain.Player>()
            .Where(x => x.Id.ToString() != output.PlayerId).ToList();
        DeltaSync(players, [player], [], SyncType.Delta);
    }

    private void DeltaSync(
        List<Player> players,
        List<Avatar> avatars,
        List<Projectile> projectiles,
        SyncType sync)
    {
        foreach (var player in players)
        {
            var output = new LocalState()
            {
                PlayerId = player.Id.ToString(),
                Sync = sync
            };
            output.Avatars.AddRange(avatars);
            output.Projectiles.AddRange(projectiles);
            _producerLS.Produce($"{_outputTopicLS}_{output.PlayerId}", output.PlayerId, output);
        }
    }

    private void MoveAi(string key, WorldChange value)
    {
        var agent = new AiAgent()
        {
            Id = value.EntityId,
            Location = value.Location,
            LastUpdate = value.GameTime
        };
        _redisBroker.UpsertAvatarLocation(new ClassLibrary.Classes.Domain.AiAgent()
        {
            Id = Guid.Parse(agent.Id),
            Location = new ClassLibrary.Classes.Data.Coordinates()
            {
                X = agent.Location.X,
                Y = agent.Location.Y
            }
        });
        _producerA.Produce(_outputTopicA, agent.Id, agent);

        var avatar = new Avatar()
        {
            Id = value.EntityId,
            Location = value.Location
        };
        
        var players = _redisBroker
            .GetEntities(agent.Location.X, agent.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [avatar], [], SyncType.Delta);
    }

    private void MoveBullet(string key, WorldChange value)
    {
        var bullet = new Projectile()
        {
            Id = value.EntityId,
            Location = value.Location,
            Direction = value.Direction,
            LastUpdate = value.GameTime,
            TimeToLive = value.Timer
        };

        SyncType sync;
        if (bullet.TimeToLive <= 0)
        {
            sync = SyncType.Delete;
        }
        else
        {
            _producerP.Produce(_outputTopicP, bullet.Id, bullet);
            sync = SyncType.Delta;
        }

        var players = _redisBroker
            .GetEntities(bullet.Location.X, bullet.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [], [bullet], sync);
    }

    private void SpawnAi(string key, WorldChange value)
    {
        var agent = new AiAgent()
        {
            Id = Guid.NewGuid().ToString(),
            Location = new Coordinates() {X = value.Location.X, Y = value.Location.Y},
            LastUpdate = DateTime.UtcNow.Ticks,
        };
        _producerA.Produce(_outputTopicA, agent.Id, agent);
        
        var avatar = new Avatar()
        {
            Id = agent.Id,
            Location = agent.Location
        };
        
        var players = _redisBroker
            .GetEntities(agent.Location.X, agent.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [avatar], [], SyncType.Delta);
    }

    private void SpawnBullet(string key, WorldChange value)
    {
        var bullet = new Projectile()
        {
            Id = value.EntityId,
            Location = value.Location,
            Direction = value.Direction,
            LastUpdate = DateTime.UtcNow.Ticks,
            TimeToLive = 100
        };
        _producerP.Produce(_outputTopicP, bullet.Id, bullet);
        
        var players = _redisBroker
            .GetEntities(bullet.Location.X, bullet.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        DeltaSync(players, [], [bullet], SyncType.Delta);
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
        DeltaSync(players, [player], [], SyncType.Delete);
    }
}