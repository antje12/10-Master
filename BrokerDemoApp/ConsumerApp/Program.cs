using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.RabbitMQ;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ConsumerApp;

class Program
{
    private const string _kafkaServers = "localhost:19092";
    private const string _groupId = "msg-group";
    private const string _schemaRegistry = "localhost:8081";

    private static CancellationTokenSource _cts;

    private static KafkaAdministrator _a;
    private static KafkaProducer _p;
    private static KafkaConsumer _c;

    private static RabbitProducer _rp;
    private static RabbitConsumer _rc;

    private static void Setup()
    {
        var adminConfig = new AdminClientConfig {BootstrapServers = _kafkaServers};
        var schemaRegistryConfig = new SchemaRegistryConfig {Url = _schemaRegistry};
        var avroSerializerConfig = new AvroSerializerConfig {BufferBytes = 100};
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

        _cts = new CancellationTokenSource();

        _a = new KafkaAdministrator(adminConfig);
        _p = new KafkaProducer(producerConfig, schemaRegistryConfig, avroSerializerConfig);
        _c = new KafkaConsumer(consumerConfig, schemaRegistryConfig, _cts);

        _rp = new RabbitProducer("output");
        IConsumer.ProcessMessage action = SendKafkaResponse;
        _rc = new RabbitConsumer("input", _cts);
    }

    private static void SendKafkaResponse(string key, string value)
    {
        _p.Produce("output", key, value);
    }

    private static void SendRabbitResponse(string key, string value)
    {
        _rp.Produce("output", key, value);
    }

    static async Task Main()
    {
        Setup();
        await KafkaRun();
        //await RabbitRun();
    }

    private static async Task KafkaRun()
    {
        Console.WriteLine("Kafka Consumer Started");
        await _a.CreateTopic("input");
        await _a.CreateTopic("output");
        
        IConsumer.ProcessMessage action = SendKafkaResponse;
        await _c.StartConsumer("input", action);
    }

    private static async Task RabbitRun()
    {
        Console.WriteLine("RabbitMQ Consumer Started");
        
        IConsumer.ProcessMessage action = SendRabbitResponse;
        await _rc.StartConsumer("input", action);
    }
}