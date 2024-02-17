using ClassLibrary.Classes.Client;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using WorldService.Interfaces;

namespace WorldService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class WorldService : BackgroundService, IConsumerService
{
    private const string GroupId = "world-group";

    private readonly KafkaAdministrator _admin;
    private readonly KafkaProducer<Output> _producer;
    private readonly KafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public WorldService()
    {
        Console.WriteLine($"WorldService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _admin.CreateTopic(KafkaTopic.World);
        _producer = new KafkaProducer<Output>(config);
        _consumer = new KafkaConsumer<Input>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"WorldService started");

        await _admin.CreateTopic(KafkaTopic.World);
        IConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(KafkaTopic.World, action, ct);

        IsRunning = false;
        Console.WriteLine($"WorldService stopped");
    }

    private void ProcessMessage(string key, Input value)
    {
        var output = new Output()
        {
            PlayerId = value.PlayerId,
            Location = value.Location
        };

        //_producer.Produce($"{KafkaTopic.LocalState}_{output.PlayerId.ToString()}", key, output);
        _producer.Produce(KafkaTopic.LocalState.ToString(), key, output);
    }
}