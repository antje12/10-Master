using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Messages;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using CollisionService.Interfaces;

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
        var avatars = _mongoBroker.ReadCloseScreen(value.ToLocation);
        foreach (var avatar in avatars)
        {
            if (value.PlayerId == avatar.Id)
            {
                continue;
            }

            if (circleCollision(value.ToLocation, avatar.Location))
            {
                return;
            }
        }
        
        //if (!IsLocationFree(value.ToLocation))
        //    return;

        var output = new WorldChange()
        {
            PlayerId = value.PlayerId,
            NewLocation = value.ToLocation
        };

        _producer.Produce(OutputTopic, key, output);
    }

    private bool circleCollision(Coordinates location1, Coordinates location2) {

        float dx = location1.X - location2.X;
        float dy = location1.Y - location2.Y;

        // a^2 + b^2 = c^2
        // c = sqrt(a^2 + b^2)
        double distance = Math.Sqrt(dx * dx + dy * dy);

        // if radius overlap
        if (distance < 25 + 25) {
            // Collision!
            return true;
        }

        return false;
    }
    
    private bool IsLocationFree(Coordinates location)
    {
        float deadZoneStartX = 100;
        float deadZoneStopX = 200;
        float deadZoneStartY = 100;
        float deadZoneStopY = 200;

        if (deadZoneStartX <= location.X && location.X <= deadZoneStopX &&
            deadZoneStartY <= location.Y && location.Y <= deadZoneStopY)
        {
            return false;
        }

        var locationContent = _mongoBroker.ReadLocation(location);

        if (locationContent != null)
        {
            return false;
        }

        return true;
    }
}