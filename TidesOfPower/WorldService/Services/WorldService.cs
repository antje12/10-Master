using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using WorldService.Interfaces;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;
using ChangeType = ClassLibrary.Messages.Protobuf.ChangeType;
using Coordinates = ClassLibrary.Messages.Protobuf.Coordinates;
using SyncType = ClassLibrary.Messages.Protobuf.SyncType;

namespace WorldService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class WorldService : BackgroundService, IConsumerService
{
    private const string GroupId = "world-group";
    private KafkaTopic InputTopic = KafkaTopic.World;
    private KafkaTopic OutputTopic = KafkaTopic.LocalState;

    private readonly KafkaAdministrator _admin;
    private readonly ProtoKafkaProducer<LocalState> _producer;
    private readonly ProtoKafkaConsumer<WorldChange> _consumer;

    private readonly MongoDbBroker _mongoBroker;   
    private readonly RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }

    public WorldService()
    {
        Console.WriteLine($"WorldService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _producer = new ProtoKafkaProducer<LocalState>(config);
        _consumer = new ProtoKafkaConsumer<WorldChange>(config);
        _mongoBroker = new MongoDbBroker();
        _redisBroker = new RedisBroker();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine($"WorldService started");
        await _admin.CreateTopic(InputTopic);
        IProtoConsumer<WorldChange>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(InputTopic, action, ct);
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

    private void MovePlayer(string key, WorldChange value)
    {
        var stopwatch = new Stopwatch();
        var s2 = new Stopwatch();
        stopwatch.Start();
        
        string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        Console.WriteLine($"Got {value.EventId} at {timestampWithMs}");
        
        var player = new Avatar()
        {
            Id = value.EntityId,
            Location = value.Location
        };
        
        s2.Start();
        //_mongoBroker.CreateEntity(player);
        _redisBroker.UpsertAvatarLocation(new ClassLibrary.Classes.Domain.Avatar()
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
        
        _producer.Produce($"{OutputTopic}_{player.Id}", key, output);

        var enemies = output.Avatars.Where(x => x.Id != output.PlayerId);
        foreach (var enemy in enemies)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = enemy.Id,
                Sync = SyncType.Delta
            };
            deltaOutput.Avatars.Add(player);
            _producer.Produce($"{OutputTopic}_{enemy.Id}", enemy.Id.ToString(), deltaOutput);
        }
        
        timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        Console.WriteLine($"Send {output.EventId} at {timestampWithMs}");
        
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        if (value.EventId != "") Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time -- {value.EventId}");
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
            Timer = 10
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
            Timer = 10
        };
        
        s2.Start();
        _redisBroker.Insert(pro);
        var avatars = _redisBroker.GetEntities(pro.Location.X, pro.Location.Y).OfType<ClassLibrary.Classes.Domain.Avatar>();
        s2.Stop();
        
        foreach (var avatar in avatars)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = avatar.Id.ToString(),
                Sync = SyncType.Delta
            };
            deltaOutput.Projectiles.Add(projectile);
            _producer.Produce($"{OutputTopic}_{avatar.Id}", avatar.Id.ToString(), deltaOutput);
        }
        
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        if (elapsedTime > 20) Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time");
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
            Timer = value.Timer
        };
        var pro = new ClassLibrary.Classes.Domain.Projectile()
        {
            Id = Guid.Parse(bullet.Id),
            Location = new ClassLibrary.Classes.Data.Coordinates()
            {
                X = bullet.Location.X,
                Y = bullet.Location.Y
            },
            Timer = bullet.Timer
        };

        SyncType sync;
        
        s2.Start();
        if (bullet.Timer <= 0)
        {
            _redisBroker.Delete(pro);
            sync = SyncType.Delete;
        }
        else
        {
            _redisBroker.UpdateProjectile(pro);
            sync = SyncType.Delta;
        }
        var entities = _redisBroker.GetEntities(pro.Location.X, pro.Location.Y).OfType<ClassLibrary.Classes.Domain.Avatar>().ToList();
        s2.Stop();
        
        foreach (var entity in entities)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = entity.Id.ToString(),
                Sync = sync
            };
            deltaOutput.Projectiles.Add(bullet);
            _producer.Produce($"{OutputTopic}_{entity.Id}", entity.Id.ToString(), deltaOutput);
        }
        
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        if (elapsedTime > 20) Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time");
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
            .OfType<ClassLibrary.Classes.Domain.Avatar>().ToList();
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
            _producer.Produce($"{OutputTopic}_{avatar.Id}", avatar.Id.ToString(), deltaOutput);
        }
        
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        if (elapsedTime > 20) Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time");
    }
}