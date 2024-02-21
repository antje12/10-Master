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
        _admin.CreateTopic(KafkaTopic.Collision);
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

        await _admin.CreateTopic(KafkaTopic.Collision);
        IConsumer<CollisionCheck>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(KafkaTopic.Collision, action, ct);

        IsRunning = false;
        Console.WriteLine($"CollisionService stopped");
    }

    private void ProcessMessage(string key, CollisionCheck value)
    {
        if (!IsLocationFree(value.ToLocation))
            return;

        var output = new WorldChange()
        {
            PlayerId = value.PlayerId,
            NewLocation = value.ToLocation
        };

        //_producer.Produce($"{KafkaTopic.LocalState}_{output.PlayerId.ToString()}", key, output);
        _producer.Produce(KafkaTopic.World.ToString(), key, output);
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

        var locationContent = _mongoBroker.ReadAvatar(location);

        if (locationContent != null)
        {
            return false;
        }

        return true;
    }
}