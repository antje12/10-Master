using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using Confluent.Kafka;

namespace KafkaConsumerApp;

class Program
{
    private const string _kafkaServers = "kafka-service:9092";
    private const string _groupId = "msg-group";

    private static CancellationTokenSource _cts;

    private static KafkaAdministrator _a;
    private static KafkaProducer _p;
    private static KafkaConsumer _c;

    static async Task Main()
    {
        Setup();
        await _a.CreateTopic("input");
        await _a.CreateTopic("output");

        await KafkaRun();
    }

    private static void Setup()
    {
        var adminConfig = new AdminClientConfig {BootstrapServers = _kafkaServers};
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
        _p = new KafkaProducer(producerConfig);
        _c = new KafkaConsumer(consumerConfig);
    }

    private static async Task KafkaRun()
    {
        Console.WriteLine("Kafka Consumer Started");
        IConsumer.ProcessMessage action = SendKafkaResponse;
        await _c.Consume("input", action, _cts.Token);
    }

    private static void SendKafkaResponse(string key, string value)
    {
        _p.Produce("output", key, value);
    }
}