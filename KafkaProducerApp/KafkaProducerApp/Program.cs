using ClassLibrary;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

class Program
{
    private const string _kafkaServers = "localhost:19092";
    private const string _schemaRegistry = "localhost:8081";

    static void Main()
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _kafkaServers,
            Acks = Acks.None,
            LingerMs = 0,
            BatchSize = 1
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

        var p = new Producer(producerConfig,
            schemaRegistryConfig,
            avroSerializerConfig);

        var topic = "msg-topic";
        var player = new PlayerPos()
        {
            ID = "MyId",
            X = 0,
            Y = 0
        };

        while (true)
        {
            string message = Console.ReadLine();
            p.Produce(topic, player.ID, $"{player.X},{player.Y}");
        }
    }
}