using System.Diagnostics;
using ClassLibrary.GameLogic;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using CollisionService.Interfaces;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;

namespace CollisionService.Services;

public class CollisionService : BackgroundService, IConsumerService
{
    private string _groupId = "collision-group";
    private KafkaTopic _inputTopic = KafkaTopic.Collision;
    private KafkaTopic _outputTopic = KafkaTopic.World;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<WorldChange> _producer;
    private ProtoKafkaConsumer<CollisionCheck> _consumer;

    private MongoDbBroker _mongoBroker;
    private RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public CollisionService()
    {
        Console.WriteLine($"CollisionService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producer = new ProtoKafkaProducer<WorldChange>(config);
        _consumer = new ProtoKafkaConsumer<CollisionCheck>(config);
        _mongoBroker = new MongoDbBroker(localTest);
        _redisBroker = new RedisBroker(localTest);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine($"CollisionService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<CollisionCheck>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine($"CollisionService stopped");
    }

    private void ProcessMessage(string key, CollisionCheck value)
    {
        var stopwatch = new Stopwatch();
        var s2 = new Stopwatch();
        stopwatch.Start();
        
        string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        Console.WriteLine($"Got {value.EventId} at {timestampWithMs}");
        
        s2.Start();
        var entities = _redisBroker.GetCloseEntities(value.ToLocation.X, value.ToLocation.Y);
        s2.Stop();
        foreach (var entity in entities)
        {
            if (value.EntityId == entity.Id.ToString())
            {
                continue;
            }

            var w1 =
                value.Entity is EntityType.Projectile ? 5 :
                value.Entity is EntityType.Player or EntityType.Ai ? 25 : 0;
            var w2 =
                entity is ClassLibrary.Classes.Domain.Projectile ? 5 :
                entity is ClassLibrary.Classes.Domain.Avatar ? 25 : 0;

            if (Collide.Circle(
                    value.ToLocation.X,value.ToLocation.Y, w1, 
                    entity.Location.X, entity.Location.Y, w2))
            {
                if (value.Entity is EntityType.Player or EntityType.Ai && entity is ClassLibrary.Classes.Domain.Avatar)
                {
                    return;
                }

                //if (value.Entity is EntityType.Avatar && entity is ClassLibrary.Classes.Domain.Projectile)
                //{
                //    Damage(Guid.Parse(value.EntityId), value.ToLocation);
                //    return;
                //}

                if (value.Entity is EntityType.Projectile && entity is ClassLibrary.Classes.Domain.Avatar)
                {
                    var coordinates = new Coordinates();
                    coordinates.X = entity.Location.X;
                    coordinates.Y = entity.Location.Y;
                    Damage(entity.Id, coordinates);
                }
            }
        }

        if (value.Entity is EntityType.Player or EntityType.Ai)
        {
            var output = new WorldChange()
            {
                EntityId = value.EntityId,
                Change = value.Entity is EntityType.Ai ? ChangeType.MoveAi : ChangeType.MovePlayer,
                Location = value.ToLocation,
                EventId = value.EventId
            };

            _producer.Produce(_outputTopic, key, output);
            timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Send {output.EventId} at {timestampWithMs}");
        }
        else if (value.Entity is EntityType.Projectile)
        {
            var output = new WorldChange()
            {
                EntityId = value.EntityId,
                Change = ChangeType.MoveBullet,
                Location = value.ToLocation,
                Timer = value.Timer,
                Direction = value.Direction,
                GameTime = value.GameTime
            };

            _producer.Produce(_outputTopic, key, output);
        }
        
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        if (value.EventId != "") Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time -- {value.EventId}");
    }

    private void Damage(Guid entityId, Coordinates entityLocation)
    {
        var output = new WorldChange()
        {
            EntityId = entityId.ToString(),
            Change = ChangeType.DamagePlayer,
            Location = entityLocation
        };
        _producer.Produce(_outputTopic, entityId.ToString(), output);
    }
}