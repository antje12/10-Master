using ClassLibrary;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

class Program
{
    private const string _kafkaServers = "localhost:19092";
    private const string _groupId = "msg-group";
    private const string _schemaRegistry = "localhost:8081";
    
    static void Main()
    {
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
        
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe("msg-topic");

        var data = new Dictionary<string, PlayerPos>();
        while (true)
        {
            var consumeResult = consumer.Consume();
            var key = consumeResult.Message.Key;
            var message = consumeResult.Message.Value;

            if (!data.ContainsKey(key))
            {
                data.Add(key, new PlayerPos()
                {
                    ID = "MyId",
                    X = 0,
                    Y = 0
                });
            }

            switch (message)
            {
                case "w":
                    data[key].Y++;
                    break;
                case "a":
                    data[key].X--;
                    break;
                case "s":
                    data[key].Y--;
                    break;
                case "d":
                    data[key].X++;
                    break;
            }

            Console.Clear();
            Console.Write($"{key}: {data[key].X},{data[key].Y}");
        }
    }
}