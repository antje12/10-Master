using System.Diagnostics;
using ClassLibrary.Classes.Domain;
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
    private KafkaTopic _outputTopicW = KafkaTopic.World;
    private KafkaTopic _outputTopicA = KafkaTopic.Ai;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<WorldChange> _producerW;
    private ProtoKafkaProducer<ClassLibrary.Messages.Protobuf.AiAgent> _producerA;
    private ProtoKafkaConsumer<CollisionCheck> _consumer;

    private MongoDbBroker _mongoBroker;
    private RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public CollisionService()
    {
        Console.WriteLine("CollisionService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producerW = new ProtoKafkaProducer<WorldChange>(config);
        _producerA = new ProtoKafkaProducer<ClassLibrary.Messages.Protobuf.AiAgent>(config);
        _consumer = new ProtoKafkaConsumer<CollisionCheck>(config);
        _mongoBroker = new MongoDbBroker(localTest);
        _redisBroker = new RedisBroker(localTest);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("CollisionService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<CollisionCheck>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("CollisionService stopped");
    }

    private void ProcessMessage(string key, CollisionCheck value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(key, value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(string key, CollisionCheck value)
    {
        var blocked = false;
        var entities = _redisBroker.GetCloseEntities(value.ToLocation.X, value.ToLocation.Y);
        foreach (var entity in entities)
        {
            if (value.EntityId == entity.Id.ToString())
                continue;

            var w1 =
                value.Entity is EntityType.Bullet ? 5 :
                value.Entity is EntityType.Player or EntityType.Ai ? 25 : 0;
            var w2 =
                entity is ClassLibrary.Classes.Domain.Projectile ? 5 :
                entity is ClassLibrary.Classes.Domain.Avatar ? 25 : 0;
            if (Collide.Circle(
                    value.ToLocation.X, value.ToLocation.Y, w1,
                    entity.Location.X, entity.Location.Y, w2))
            {
                switch (value.Entity)
                {
                    case EntityType.Player:
                    case EntityType.Ai:
                        if (entity is ClassLibrary.Classes.Domain.Avatar)
                            blocked = true;
                        break;
                    case EntityType.Bullet:
                        if (entity is ClassLibrary.Classes.Domain.Avatar)
                            DamageAvatar(entity);
                        break;
                }
            }
        }

        var output = new WorldChange()
        {
            EntityId = value.EntityId,
            Location = value.ToLocation
        };
        switch (value.Entity)
        {
            case EntityType.Player:
                if (blocked)
                {
                    return;
                }
                output.Change = ChangeType.MovePlayer;
                output.EventId = value.EventId;
                break;
            case EntityType.Ai:
                if (blocked)
                {
                    KeepAiAlive(key, value);
                    return;
                }
                output.Change = ChangeType.MoveAi;
                output.EventId = value.EventId;
                break;
            case EntityType.Bullet:
                output.Change = ChangeType.MoveBullet;
                output.Timer = value.Timer;
                output.Direction = value.Direction;
                output.GameTime = value.GameTime;
                break;
        }
        _producerW.Produce(_outputTopicW, key, output);
    }

    private void KeepAiAlive(string key, CollisionCheck value)
    {
        var output = new ClassLibrary.Messages.Protobuf.AiAgent()
        {
            Id = value.EntityId,
            Location = value.FromLocation,
            LastUpdate = value.GameTime,
        };
        _producerA.Produce(_outputTopicA, key, output);
    }

    private void DamageAvatar(Entity entity)
    {
        var output = new WorldChange()
        {
            EntityId = entity.Id.ToString(),
            Change = ChangeType.DamageAgent,
            Location = new Coordinates()
            {
                X = entity.Location.X,
                Y = entity.Location.Y
            }
        };
        _producerW.Produce(_outputTopicW, output.EntityId, output);
    }
}