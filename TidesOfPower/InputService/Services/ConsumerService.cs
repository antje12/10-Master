using ClassLibrary.Classes;
using ClassLibrary.Classes.Client;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using InputService.Interfaces;

namespace InputService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class ConsumerService : BackgroundService, IConsumerService
{
    private const string InputTopic = "input";
    private const string OutputTopic = "output";
    private const string GroupId = "msg-group";
    private const string KafkaServers = "localhost:19092";
    private const string SchemaRegistry = "localhost:8081";

    private readonly SchemaRegistryConfig _schemaRegistryConfig = new()
    {
        Url = SchemaRegistry
    };
    
    private readonly AvroSerializerConfig _avroSerializerConfig = new()
    {
        BufferBytes = 100
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
    private readonly KafkaProducer<Output> _producer;
    private readonly KafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public ConsumerService()
    {
        Console.WriteLine($"ConsumerService created");
        _admin = new KafkaAdministrator(_adminConfig);
        _producer = new KafkaProducer<Output>(_producerConfig, _schemaRegistryConfig, _avroSerializerConfig);
        _consumer = new KafkaConsumer<Input>(_consumerConfig, _schemaRegistryConfig);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"ConsumerService started");

        await _admin.CreateTopic(InputTopic);
        IConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(InputTopic, action, ct);

        IsRunning = false;
        Console.WriteLine($"ConsumerService stopped");
    }

    private void ProcessMessage(string key, Input value)
    {
        var output = new Output()
        {
            PlayerId = value.PlayerId,
            Location = value.Location
        };
        foreach (var input in value.KeyInput)
        {
            switch (input)
            {
                case GameKey.Up:
                    output.Location.Y -= 100 * (float) value.Timer;
                    break;
                case GameKey.Down:
                    output.Location.Y += 100 * (float) value.Timer;
                    break;
                case GameKey.Left:
                    output.Location.X -= 100 * (float) value.Timer;
                    break;
                case GameKey.Right:
                    output.Location.X += 100 * (float) value.Timer;
                    break;
                case GameKey.Attack:
                case GameKey.Interact:
                default:
                    break;
            }
        }

        _producer.Produce(OutputTopic, key, output);
    }
}