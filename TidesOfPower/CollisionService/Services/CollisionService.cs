using System.Diagnostics;
using ClassLibrary.Domain;
using ClassLibrary.GameLogic;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using CollisionService.Interfaces;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;
using EntityType = ClassLibrary.Messages.Protobuf.EntityType;

namespace CollisionService.Services;

public class CollisionService : BackgroundService, IConsumerService
{
    private string _groupId = "collision-group";
    private KafkaTopic _inputTopic = KafkaTopic.Collision;
    private KafkaTopic _outputTopicW = KafkaTopic.World;
    private KafkaTopic _outputTopicA = KafkaTopic.Ai;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<World_M> _producerW;
    private ProtoKafkaProducer<Ai_M> _producerA;
    private ProtoKafkaConsumer<Collision_M> _consumer;

    private MongoDbBroker _mongoBroker;
    private RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public CollisionService()
    {
        Console.WriteLine("CollisionService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producerW = new ProtoKafkaProducer<World_M>(config);
        _producerA = new ProtoKafkaProducer<Ai_M>(config);
        _consumer = new ProtoKafkaConsumer<Collision_M>(config);
        _mongoBroker = new MongoDbBroker(localTest);
        _redisBroker = new RedisBroker(localTest);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("CollisionService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<Collision_M>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("CollisionService stopped");
    }

    private void ProcessMessage(string key, Collision_M value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(key, value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(string key, Collision_M value)
    {
        if (!string.IsNullOrEmpty(value.EventId))
        {
            string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Got {value.EventId} at {timestampWithMs}");
        }
        var blocked = false;
        var entities = _redisBroker.GetCloseEntities(value.ToLocation.X, value.ToLocation.Y);
        foreach (var entity in entities)
        {
            if (value.EntityId == entity.Id.ToString())
                continue;

            var w1 =
                value.EntityType is EntityType.Bullet ? 5 :
                value.EntityType is EntityType.Player or EntityType.Ai ? 25 : 0;
            var w2 =
                entity is Projectile ? 5 :
                entity is Agent ? 25 : 0;
            if (Collide.Circle(
                    value.ToLocation.X, value.ToLocation.Y, w1,
                    entity.Location.X, entity.Location.Y, w2))
            {
                switch (value.EntityType)
                {
                    case EntityType.Player:
                    case EntityType.Ai:
                        if (entity is Agent)
                            blocked = true;
                        break;
                    case EntityType.Bullet:
                        if (entity is Agent)
                            DamageAvatar(entity);
                        break;
                }
            }
        }

        var msgOut = new World_M()
        {
            EntityId = value.EntityId,
            Location = value.ToLocation
        };
        switch (value.EntityType)
        {
            case EntityType.Player:
                if (blocked)
                {
                    return;
                }
                msgOut.Change = Change.MovePlayer;
                msgOut.EventId = value.EventId;
                break;
            case EntityType.Ai:
                if (blocked)
                {
                    KeepAiAlive(key, value);
                    return;
                }
                msgOut.Change = Change.MoveAi;
                msgOut.LastUpdate = value.LastUpdate;
                break;
            case EntityType.Bullet:
                msgOut.Change = Change.MoveBullet;
                msgOut.Direction = value.Direction;
                msgOut.LastUpdate = value.LastUpdate;
                msgOut.TTL = value.TTL;
                break;
        }
        _producerW.Produce(_outputTopicW, key, msgOut);
        if (!string.IsNullOrEmpty(value.EventId))
        {
            string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Sent {value.EventId} at {timestampWithMs}");
        }
    }

    private void KeepAiAlive(string key, Collision_M value)
    {
        var msgOut = new Ai_M()
        {
            Id = value.EntityId,
            Location = value.FromLocation,
            LastUpdate = value.LastUpdate
        };
        _producerA.Produce(_outputTopicA, key, msgOut);
    }

    private void DamageAvatar(Entity entity)
    {
        var output = new World_M()
        {
            EntityId = entity.Id.ToString(),
            Change = Change.DamageAgent,
            Location = new Coordinates_M()
            {
                X = entity.Location.X,
                Y = entity.Location.Y
            }
        };
        _producerW.Produce(_outputTopicW, output.EntityId, output);
    }
}