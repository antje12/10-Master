using ClassLibrary.Kafka;
using ClassLibrary.RabbitMQ;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ProducerApp;

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

        _rp = new RabbitProducer("input");
        _rc = new RabbitConsumer("output", _cts);
    }

    private static Dictionary<string, int> _results;

    static async Task Main()
    {
        Setup();
        //await KafkaRun(1000);
        await RabbitRun(1000);
    }

    private static async Task KafkaRun(int runs)
    {
        Console.WriteLine("Kafka Producer Started");
        await _a.CreateTopic("input");
        await _a.CreateTopic("output");

        Task.Factory.StartNew(() => _c.StartConsumer("output", (key, message) => { }));
        while (true)
        {
            string value = Console.ReadLine();
            _p.Produce("input", "key", value);
        }
    }

    private static async Task RabbitRun(int runs)
    {
        Console.WriteLine("RabbitMQ Producer Started");
        
        Task.Factory.StartNew(() => _rc.StartConsumer("output", (key, message) => { }));
        while (true)
        {
            string value = Console.ReadLine();
            _rp.Produce("input", "key", value);
        }
    }
}