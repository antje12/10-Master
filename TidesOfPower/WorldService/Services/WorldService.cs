using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using WorldService.Interfaces;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;
using Google.Protobuf.WellKnownTypes;
using ChangeType = ClassLibrary.Messages.Protobuf.ChangeType;
using Coordinates = ClassLibrary.Messages.Protobuf.Coordinates;
using SyncType = ClassLibrary.Messages.Protobuf.SyncType;

namespace WorldService.Services;

public class WorldService : BackgroundService, IConsumerService
{
    private string _groupId = "world-group";
    private KafkaTopic _inputTopic = KafkaTopic.World;
    private KafkaTopic _outputTopicL = KafkaTopic.LocalState;
    private KafkaTopic _outputTopicP = KafkaTopic.Projectile;
    private KafkaTopic _outputTopicA = KafkaTopic.Ai;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<LocalState> _producerL;
    private ProtoKafkaProducer<Projectile> _producerP;
    private ProtoKafkaProducer<AiAgent> _producerA;
    private ProtoKafkaConsumer<WorldChange> _consumer;

    private MongoDbBroker _mongoBroker;
    private RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = false;

    public WorldService()
    {
        Console.WriteLine($"WorldService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producerL = new ProtoKafkaProducer<LocalState>(config);
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
        Console.WriteLine($"WorldService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<WorldChange>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine($"WorldService stopped");
    }

    private void ProcessMessage(string key, WorldChange value)
    {
        switch (value.Change)
        {
            case ChangeType.MovePlayer:
                MovePlayer(key, value);
                break;
            case ChangeType.SpawnAi:
                SpawnAi(key, value);
                break;
            case ChangeType.MoveAi:
                MoveAi(key, value);
                break;
            case ChangeType.SpawnBullet:
                SpawnBullet(key, value);
                break;
            case ChangeType.MoveBullet:
                MoveBullet(key, value);
                break;
            case ChangeType.DamagePlayer:
                DamagePlayer(key, value);
                break;
        }
    }

    private void SpawnAi(string key, WorldChange value)
    {
        var agent = new AiAgent()
        {
            Id = Guid.NewGuid().ToString(),
            Location = new Coordinates() {X = value.Location.X, Y = value.Location.Y},
            LastUpdate = Timestamp.FromDateTime(DateTime.UtcNow), // Timestamp requires UTC DateTime
        };
        _producerA.Produce(_outputTopicA, agent.Id, agent);
    }

    private void MoveAi(string key, WorldChange value)
    {
        var agent = new AiAgent()
        {
            Id = value.EntityId,
            Location = value.Location,
            LastUpdate = Timestamp.FromDateTime(DateTime.UtcNow), // Timestamp requires UTC DateTime
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
        var entities = _redisBroker.GetEntities(agent.Location.X, agent.Location.Y);
        var avatars = entities.OfType<ClassLibrary.Classes.Domain.Player>().Select(a => new Avatar()
        {
            Id = a.Id.ToString(),
            Location = new Coordinates()
            {
                X = a.Location.X,
                Y = a.Location.Y,
            }
        }).ToList();

        _producerA.Produce(_outputTopicA, agent.Id, agent);

        var a = new Avatar()
        {
            Id = value.EntityId,
            Location = value.Location
        };

        var enemies = avatars.Where(x => x.Id != agent.Id);
        foreach (var enemy in enemies)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = enemy.Id,
                Sync = SyncType.Delta
            };
            deltaOutput.Avatars.Add(a);
            _producerL.Produce($"{_outputTopicL}_{enemy.Id}", enemy.Id.ToString(), deltaOutput);
        }
    }

    private void MovePlayer(string key, WorldChange value)
    {
        var stopwatch = new Stopwatch();
        var s2 = new Stopwatch();
        stopwatch.Start();

        string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        //Console.WriteLine($"Got {value.EventId} at {timestampWithMs}");

        var player = new Avatar()
        {
            Id = value.EntityId,
            Location = value.Location
        };

        s2.Start();
        //_mongoBroker.CreateEntity(player);
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
        s2.Stop();
        var output = new LocalState()
        {
            PlayerId = player.Id,
            Sync = SyncType.Full,
            EventId = value.EventId
        };

        var avatars = entities.OfType<ClassLibrary.Classes.Domain.Avatar>().Select(a => new Avatar()
        {
            Id = a.Id.ToString(),
            Location = new Coordinates()
            {
                X = a.Location.X,
                Y = a.Location.Y,
            }
        }).ToList();
        var projectiles = entities.OfType<ClassLibrary.Classes.Domain.Projectile>().Select(p => new Projectile()
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

        _producerL.Produce($"{_outputTopicL}_{player.Id}", key, output);

        var enemies = entities.OfType<ClassLibrary.Classes.Domain.Player>().Where(x => x.Id.ToString() != output.PlayerId);
        foreach (var enemy in enemies)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = enemy.Id.ToString(),
                Sync = SyncType.Delta
            };
            deltaOutput.Avatars.Add(player);
            _producerL.Produce($"{_outputTopicL}_{enemy.Id}", enemy.Id.ToString(), deltaOutput);
        }

        timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        //Console.WriteLine($"Send {output.EventId} at {timestampWithMs}");

        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //if (value.EventId != "") Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time -- {value.EventId}");
    }

    private void SpawnBullet(string key, WorldChange value)
    {
        var stopwatch = new Stopwatch();
        var s2 = new Stopwatch();
        stopwatch.Start();

        //var avatars = _mongoBroker.ReadScreen(value.Location);
        var projectile = new Projectile()
        {
            Id = value.EntityId,
            Location = value.Location,
            Direction = value.Direction,
            LastUpdate = Timestamp.FromDateTime(DateTime.UtcNow), // Timestamp requires UTC DateTime
            TimeToLive = 100
        };
        var pro = new ClassLibrary.Classes.Domain.Projectile()
        {
            Id = Guid.Parse(projectile.Id),
            Location = new ClassLibrary.Classes.Data.Coordinates()
            {
                X = value.Location.X,
                Y = value.Location.Y
            },
            Direction = new ClassLibrary.Classes.Data.Coordinates()
            {
                X = value.Direction.X,
                Y = value.Direction.Y
            },
            TimeToLive = 10
        };

        _producerP.Produce(_outputTopicP, projectile.Id, projectile);

        s2.Start();
        //_redisBroker.Insert(pro);
        var avatars = _redisBroker.GetEntities(pro.Location.X, pro.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>();
        s2.Stop();

        foreach (var avatar in avatars)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = avatar.Id.ToString(),
                Sync = SyncType.Delta
            };
            deltaOutput.Projectiles.Add(projectile);
            _producerL.Produce($"{_outputTopicL}_{avatar.Id}", avatar.Id.ToString(), deltaOutput);
        }

        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //if (elapsedTime > 20) Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time");
    }

    private void MoveBullet(string key, WorldChange value)
    {
        var stopwatch = new Stopwatch();
        var s2 = new Stopwatch();
        stopwatch.Start();

        var bullet = new Projectile()
        {
            Id = value.EntityId,
            Location = value.Location,
            Direction = value.Direction,
            LastUpdate = Timestamp.FromDateTime(DateTime.UtcNow), // Timestamp requires UTC DateTime
            TimeToLive = value.Timer
        };
        var pro = new ClassLibrary.Classes.Domain.Projectile()
        {
            Id = Guid.Parse(bullet.Id),
            Location = new ClassLibrary.Classes.Data.Coordinates()
            {
                X = bullet.Location.X,
                Y = bullet.Location.Y
            },
            TimeToLive = bullet.TimeToLive
        };

        SyncType sync;

        s2.Start();
        if (bullet.TimeToLive <= 0)
        {
            //_redisBroker.Delete(pro);
            sync = SyncType.Delete;
        }
        else
        {
            _producerP.Produce(_outputTopicP, bullet.Id, bullet);
            //_redisBroker.UpdateProjectile(pro);
            sync = SyncType.Delta;
        }

        var entities = _redisBroker.GetEntities(pro.Location.X, pro.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        s2.Stop();

        foreach (var entity in entities)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = entity.Id.ToString(),
                Sync = sync
            };
            deltaOutput.Projectiles.Add(bullet);
            _producerL.Produce($"{_outputTopicL}_{entity.Id}", entity.Id.ToString(), deltaOutput);
        }

        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time");
    }

    private void DamagePlayer(string key, WorldChange value)
    {
        var stopwatch = new Stopwatch();
        var s2 = new Stopwatch();
        stopwatch.Start();

        var player = new Avatar()
        {
            Id = value.EntityId
        };

        s2.Start();
        var avatars = _redisBroker.GetEntities(value.Location.X, value.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Player>().ToList();
        _redisBroker.Delete(new ClassLibrary.Classes.Domain.Avatar()
        {
            Id = Guid.Parse(player.Id)
        });
        s2.Stop();

        foreach (var avatar in avatars)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = avatar.Id.ToString(),
                Sync = SyncType.Delete
            };
            var a = new Avatar()
            {
                Id = player.Id
            };
            deltaOutput.Avatars.Add(a);
            _producerL.Produce($"{_outputTopicL}_{avatar.Id}", avatar.Id.ToString(), deltaOutput);
        }

        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //if (elapsedTime > 20) Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time");
    }
}