using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using InputService.Interfaces;

namespace InputService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class ConsumerService : BackgroundService, IConsumerService
{
    private const string Topic = "input";
    private const string GroupId = "msg-group";
    private const string KafkaServers = "localhost:19092";
    private const string SchemaRegistry = "localhost:8081";

    private readonly SchemaRegistryConfig _schemaRegistryConfig = new()
    {
        Url = SchemaRegistry
    };

    private readonly AdminClientConfig _adminConfig = new()
    {
        BootstrapServers = KafkaServers
    };

    private readonly ProducerConfig _producerConfig = new()
    {
        BootstrapServers = KafkaServers,
        Acks = Acks.None,
        LingerMs = 0,
        BatchSize = 1
    };

    private readonly ConsumerConfig _consumerConfig = new()
    {
        BootstrapServers = KafkaServers,
        GroupId = GroupId,
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    private readonly KafkaAdministrator _admin;
    private readonly KafkaProducer _producer;
    private readonly KafkaConsumer _consumer;

    public bool IsRunning { get; private set; }

    public ConsumerService()
    {
        Console.WriteLine($"ConsumerService created");
        _admin = new KafkaAdministrator(_adminConfig);
        _producer = new KafkaProducer(_producerConfig, _schemaRegistryConfig);
        _consumer = new KafkaConsumer(_consumerConfig, _schemaRegistryConfig);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"ConsumerService started");

        await _admin.CreateTopic(Topic);
        IConsumer.ProcessMessage action = null;
        await _consumer.Consume(Topic, action, ct);

        IsRunning = false;
        Console.WriteLine($"ConsumerService stopped");
    }
}