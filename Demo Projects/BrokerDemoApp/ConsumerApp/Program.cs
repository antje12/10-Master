using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
//using ClassLibrary.RabbitMQ;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ConsumerApp;

class Program
{
    private const string _kafkaServers = "kafka-1:9092";
    private const string _groupId = "msg-group";
    private const string _schemaRegistry = "http://schema-registry:8081";

    private static CancellationTokenSource _cts;

    private static KafkaAdministrator _a;
    private static KafkaProducer _p;
    private static KafkaConsumer _c;

    //private static RabbitProducer _rp;
    //private static RabbitConsumer _rc;

    private class coord
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    private static Dictionary<string, coord> _state;

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

        //_rp = new RabbitProducer("output");
        IConsumer.ProcessMessage action = SendKafkaResponse;
        //_rc = new RabbitConsumer("input", _cts);

        _state = new Dictionary<string, coord>();
    }

    private static void SendKafkaResponse(string key, string value)
    {
        if (!_state.ContainsKey(key))
        {
            _state.Add(key, new coord{X = 0, Y = 0});
        }
        switch (value)
        {
            case "up":
                _state[key].Y += 1;
                break;
            case "down":
                _state[key].Y -= 1;
                break;
            case "left":
                _state[key].X -= 1;
                break;
            case "right":
                _state[key].X += 1;
                break;
        }
        value = $"{_state[key].X}.{_state[key].Y}";
        _p.Produce("output", key, value);
    }

    private static void SendRabbitResponse(string key, string value)
    {
        //_rp.Produce("output", key, value);
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
        //Console.WriteLine("RabbitMQ Consumer Started");
        //
        //IConsumer.ProcessMessage action = SendRabbitResponse;
        //await _rc.StartConsumer("input", action);
    }
}