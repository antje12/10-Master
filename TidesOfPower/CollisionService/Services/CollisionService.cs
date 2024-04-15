using System.Diagnostics;
using ClassLibrary.Domain;
using ClassLibrary.GameLogic;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
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
    private KafkaProducer<World_M> _producerW;
    private KafkaProducer<Ai_M> _producerA;
    private KafkaConsumer<Collision_M> _consumer;

    private RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public CollisionService()
    {
        Console.WriteLine("CollisionService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producerW = new KafkaProducer<World_M>(config);
        _producerA = new KafkaProducer<Ai_M>(config);
        _consumer = new KafkaConsumer<Collision_M>(config);
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
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(string key, Collision_M value)
    {
        var treasure = 0;
        var entities = _redisBroker.GetCloseEntities(value.ToLocation.X, value.ToLocation.Y);
        var stopFlow = HandleCollisions(value, entities, out treasure);

        if (stopFlow)
        {
            if (value.EntityType == EntityType.Ai)
                KeepAiAlive(key, value);
            return;
        }

        var msgOut = new World_M()
        {
            EntityId = value.EntityId,
            Location = value.ToLocation
        };
        switch (value.EntityType)
        {
            case EntityType.Player:
                msgOut.Change = Change.MovePlayer;
                msgOut.EventId = value.EventId;
                msgOut.Value = treasure;
                break;
            case EntityType.Ai:
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
    }

    private bool HandleCollisions(Collision_M value, List<Entity> entities, out int treasure)
    {
        treasure = 0;
        var stopFlow = false;
        var valueRadius =
            value.EntityType is EntityType.Bullet ? Projectile.TypeRadius :
            value.EntityType is EntityType.Player or EntityType.Ai ? Agent.TypeRadius : 0;

        foreach (var entity in entities)
        {
            if (value.EntityId == entity.Id.ToString())
                continue;

            if (Collide.Circle(
                    value.ToLocation.X, value.ToLocation.Y, valueRadius,
                    entity.Location.X, entity.Location.Y, entity.Radius))
            {
                switch (value.EntityType)
                {
                    case EntityType.Player:
                        stopFlow |= PlayerCollision(entity, out treasure);
                        break;
                    case EntityType.Ai:
                        stopFlow |= AICollision(entity);
                        break;
                    case EntityType.Bullet:
                        stopFlow |= ProjectileCollision(entity);
                        break;
                }
            }
        }

        return stopFlow;
    }

    private bool PlayerCollision(Entity entity, out int treasure)
    {
        treasure = 0;
        if (entity is Treasure t)
        {
            treasure = t.Value;
            CollectTreasure(entity);
        }
        else if (entity is Agent)
            return true;
        return false;
    }

    private bool AICollision(Entity entity)
    {
        if (entity is Agent)
            return true;
        return false;
    }

    private bool ProjectileCollision(Entity entity)
    {
        if (entity is Agent)
            DamageAgent(entity);
        return false;
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

    private void DamageAgent(Entity entity)
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

    private void CollectTreasure(Entity entity)
    {
        var output = new World_M()
        {
            EntityId = entity.Id.ToString(),
            Change = Change.CollectTreasure,
            Location = new Coordinates_M()
            {
                X = entity.Location.X,
                Y = entity.Location.Y
            }
        };
        _producerW.Produce(_outputTopicW, output.EntityId, output);
    }
}