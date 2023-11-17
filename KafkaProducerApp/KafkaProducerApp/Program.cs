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
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();
        
        while (true)
        {
            string message = Console.ReadLine();
            Console.WriteLine($"{message} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            
            producer.Produce("msg-topic", new Message<string, string>
            {
                Key = "myPlayer",
                Value = message
            });
            
            producer.Flush();
        }
    }
}