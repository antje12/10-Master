using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.RabbitMQ;
using Confluent.Kafka;

namespace ProducerApp;

class Program
{
    private const string _kafkaServers = "localhost:9092";
    private const string _groupId = "msg-group";

    private static CancellationTokenSource _cts;

    private static KafkaAdministrator _a;
    private static KafkaProducer _p;
    private static KafkaConsumer _c;

    private static RabbitProducer _rp;
    private static RabbitConsumer _rc;

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
        _c = new KafkaConsumer(consumerConfig, _cts);

        _rp = new RabbitProducer("input");
        _rc = new RabbitConsumer("output", _cts);
    }

    private class Timer
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    private static Dictionary<int, Timer> _results;

    private static void TakeTime(string key, string value)
    {
        // update list
        _results[int.Parse(value)].To = DateTime.Now;
    }

    static async Task Main()
    {
        Setup();
        await _a.CreateTopic("input");
        await _a.CreateTopic("output");

        _results = new Dictionary<int, Timer>();
        await KafkaRun(100);
        await RabbitRun(100);
        Thread.Sleep(1000);

        _results = new Dictionary<int, Timer>();
        await KafkaRun(1000);
        await RabbitRun(1000);
    }

    private static async Task KafkaRun(int runs)
    {
        Console.WriteLine("Kafka Producer Started");
        IConsumer.ProcessMessage action = TakeTime;
        Task.Factory.StartNew(() => _c.StartConsumer("output", action));

        for (var i = 0; i < runs; i++)
        {
            // save in list
            _results.Add(i, new Timer()
            {
                From = DateTime.Now
            });
            _p.Produce("input", "key", "" + i);
            Thread.Sleep(100);
        }

        // calculate average
        var average = _results.Select(x => (x.Value.To - x.Value.From).TotalMilliseconds).Average();
        Console.WriteLine("KafkaRun Average: " + average);
    }

    private static async Task RabbitRun(int runs)
    {
        Console.WriteLine("RabbitMQ Producer Started");
        IConsumer.ProcessMessage action = TakeTime;
        Task.Factory.StartNew(() => _rc.StartConsumer("output", action));

        for (var i = 0; i < runs; i++)
        {
            // save in list
            _results.Add(i, new Timer()
            {
                From = DateTime.Now
            });
            _rp.Produce("input", "key", "" + i);
            Thread.Sleep(100);
        }

        // calculate average
        var average = _results.Select(x => (x.Value.To - x.Value.From).TotalMilliseconds).Average();
        Console.WriteLine("RabbitRun Average: " + average);
    }
}