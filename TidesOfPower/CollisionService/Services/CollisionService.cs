using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using CollisionService.Interfaces;
using ClassLibrary.Messages.Avro;

namespace CollisionService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class CollisionService : BackgroundService, IConsumerService
{
    private const string GroupId = "collision-group";
    private KafkaTopic InputTopic = KafkaTopic.Collision;
    private KafkaTopic OutputTopic = KafkaTopic.World;

    private readonly KafkaAdministrator _admin;
    private readonly KafkaProducer<WorldChange> _producer;
    private readonly KafkaConsumer<CollisionCheck> _consumer;

    private readonly MongoDbBroker _mongoBroker;

    public bool IsRunning { get; private set; }

    public CollisionService()
    {
        Console.WriteLine($"CollisionService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _producer = new KafkaProducer<WorldChange>(config);
        _consumer = new KafkaConsumer<CollisionCheck>(config);
        _mongoBroker = new MongoDbBroker();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"CollisionService started");

        await _admin.CreateTopic(InputTopic);
        IConsumer<CollisionCheck>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(InputTopic, action, ct);

        IsRunning = false;
        Console.WriteLine($"CollisionService stopped");
    }

    private void ProcessMessage(string key, CollisionCheck value)
    {
        var entities = _mongoBroker.GetCloseEntities(value.ToLocation);
        foreach (var entity in entities)
        {
            if (value.EntityId == entity.Id)
            {
                continue;
            }

            var w1 =
                value.Entity is EntityType.Projectile ? 5 :
                value.Entity is EntityType.Avatar ? 25 : 0;
            var w2 =
                entity is Projectile ? 5 :
                entity is Avatar ? 25 : 0;

            if (circleCollision(value.ToLocation, w1, entity.Location, w2))
            {
                if (value.Entity is EntityType.Avatar && entity is Avatar)
                {
                    return;
                }

                if (value.Entity is EntityType.Avatar && entity is Projectile)
                {
                    Damage(value.EntityId, value.ToLocation);
                    return;
                }

                if (value.Entity is EntityType.Projectile && entity is Avatar)
                {
                    Damage(entity.Id, entity.Location);
                }
            }
        }

        if (value.Entity is EntityType.Avatar)
        {
            var output = new WorldChange()
            {
                EntityId = value.EntityId,
                Change = ChangeType.MovePlayer,
                Location = value.ToLocation
            };

            _producer.Produce(OutputTopic, key, output);
        }
        else
        {
            var output = new WorldChange()
            {
                EntityId = value.EntityId,
                Change = ChangeType.MoveBullet,
                Location = value.ToLocation,
                Timer = value.Timer
            };

            _producer.Produce(OutputTopic, key, output);
        }
    }

    private bool circleCollision(Coordinates e1, int w1, Coordinates e2, int w2)
    {
        float dx = e1.X - e2.X;
        float dy = e1.Y - e2.Y;

        // a^2 + b^2 = c^2
        // c = sqrt(a^2 + b^2)
        double distance = Math.Sqrt(dx * dx + dy * dy);

        // if radius overlap
        if (distance < w1 + w2)
        {
            // Collision!
            return true;
        }

        return false;
    }

    private void Damage(Guid entityId, Coordinates entityLocation)
    {
        var output = new WorldChange()
        {
            EntityId = entityId,
            Change = ChangeType.DamagePlayer,
            Location = entityLocation
        };

        _producer.Produce(OutputTopic, entityId.ToString(), output);
    }
}