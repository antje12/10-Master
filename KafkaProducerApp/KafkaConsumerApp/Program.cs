using ClassLibrary;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

class Program
{
    private const string _kafkaServers = "localhost:19092";
    private const string _groupId = "msg-group";
    private const string _schemaRegistry = "localhost:8081";

    private static Administrator _a;
    private static Producer _p;
    private static Consumer _c;

    private static void SendResponse(string key, string value) {
        _p.Produce("output", "key", $"message");
    }
    
    static async Task Main()
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
        
        _a = new Administrator(adminConfig);
        await _a.Setup("input");
        await _a.Setup("output");
        
        _p = new Producer(producerConfig,
            schemaRegistryConfig,
            avroSerializerConfig);
        
        var cancellationTokenSource = new CancellationTokenSource();
        _c = new Consumer(consumerConfig,
            schemaRegistryConfig,
            cancellationTokenSource);

        Consumer.OnMessage action = SendResponse;
        await _c.StartConsumer("input", action);
    }
}