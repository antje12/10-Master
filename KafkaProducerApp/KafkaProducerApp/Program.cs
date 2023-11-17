using Confluent.Kafka;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");

        ProducerConfig producerConfig;

        const string KafkaServers = "localhost:19092";
        
        producerConfig = new ProducerConfig
        {
            BootstrapServers = KafkaServers,
            Acks = Acks.None,
            LingerMs = 0,
            BatchSize = 1
        };

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