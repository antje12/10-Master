using Confluent.Kafka;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");

        ConsumerConfig consumerConfig;

        const string KafkaServers = "localhost:19092";
        const string GroupId = "msg-group";

        consumerConfig = new ConsumerConfig
        {
            BootstrapServers = KafkaServers,
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var data = new Dictionary<string, Location>();

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe("msg-topic");

        while (true)
        {
            var consumeResult = consumer.Consume();
            var key = consumeResult.Message.Key;
            var message = consumeResult.Message.Value;

            if (!data.ContainsKey(key))
            {
                data.Add(key, new Location()
                {
                    x = 0,
                    y = 0
                });
            }

            switch (message)
            {
                case "w":
                    data[key].y++;
                    break;
                case "a":
                    data[key].x--;
                    break;
                case "s":
                    data[key].y--;
                    break;
                case "d":
                    data[key].x++;
                    break;
            }

            Console.Clear();
            Console.Write($"{key}: {data[key].x},{data[key].y}");
        }
    }
}

public class Location
{
    public int x { get; set; }
    public int y { get; set; }
}