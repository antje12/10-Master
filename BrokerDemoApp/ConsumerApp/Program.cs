using ClassLibrary;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

class Program
{
    private const string _kafkaServers = "localhost:19092";
    private const string _groupId = "msg-group";
    private const string _schemaRegistry = "localhost:8081";

    private static KafkaAdministrator _a;
    private static KafkaProducer _p;
    private static KafkaConsumer _c;

    private static RabbitProducer _rp;
    private static RabbitConsumer _rc;

    private static void SendKafkaResponse(string key, string value)
    {
        _p.Produce("output", key, value);
    }

    private static void SendRabbitResponse(string key, string value)
    {
        _rp.Produce("output", value);
    }

    static async Task Main()
    {
        await KafkaRun();
        //RabbitRun();
    }

    private static async Task KafkaRun()
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = _kafkaServers
        };
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _kafkaServers,
            Acks = Acks.None,
            LingerMs = 0,
            BatchSize = 1
        };
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = _schemaRegistry
        };
        var avroSerializerConfig = new AvroSerializerConfig
        {
            BufferBytes = 100
        };

        Console.WriteLine("Hello, World!");

        _a = new KafkaAdministrator(adminConfig);
        await _a.Setup("input");
        await _a.Setup("output");

        _p = new KafkaProducer(producerConfig,
            schemaRegistryConfig,
            avroSerializerConfig);

        var cancellationTokenSource = new CancellationTokenSource();
        _c = new KafkaConsumer(consumerConfig,
            schemaRegistryConfig,
            cancellationTokenSource);

        KafkaConsumer.OnMessage action = SendKafkaResponse;
        await _c.StartConsumer("input", action);
    }

    private static void RabbitRun()
    {
        var cts = new CancellationTokenSource();
        _rp = new RabbitProducer();
        _rc = new RabbitConsumer();
        RabbitConsumer.OnMessage ac = SendRabbitResponse;
        _rc.StartConsumer("input", ac);
    }
}