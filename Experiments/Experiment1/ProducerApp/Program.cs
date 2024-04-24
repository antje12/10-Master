using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.RabbitMQ;
using Confluent.Kafka;

namespace ProducerApp;

class Program
{
    //http://34.32.47.73:30080/InputService/Version
    private const string _kafkaServers = "localhost:19092";
    private const string _rabbitMqServers = "localhost";
    private const string _groupId = "msg-group";

    private static CancellationTokenSource _cts;

    private static KafkaAdministrator _a;
    private static KafkaProducer _p;
    private static KafkaConsumer _c;

    private static RabbitProducer _rp;
    private static RabbitConsumer _rc;

    private static List<long> _results;
    private static Guid _last;
    
    private static void Setup()
    {
        var adminConfig = new AdminClientConfig {BootstrapServers = _kafkaServers};
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _kafkaServers,
            AllowAutoCreateTopics = false,
            Acks = Acks.None,
            LingerMs = 0,
            BatchSize = 1
        };
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaServers,
            AllowAutoCreateTopics = false,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SessionTimeoutMs = 6000,
            ConsumeResultFields = "none"
        };

        _cts = new CancellationTokenSource();

        _a = new KafkaAdministrator(adminConfig);
        _p = new KafkaProducer(producerConfig);
        _c = new KafkaConsumer(consumerConfig);

        _rp = new RabbitProducer(_rabbitMqServers, "input");
        _rc = new RabbitConsumer(_rabbitMqServers, "output");
    }

    static async Task Main()
    {
        Setup();
        await _a.CreateTopic("input");
        await _a.CreateTopic("output");

        _results = new List<long>();
        _cts = new CancellationTokenSource();
        await KafkaRun(110);
        
        _results = new List<long>();
        _cts = new CancellationTokenSource();
        await RabbitRun(110);
    }

    private static async Task KafkaRun(int runs)
    {
        Console.WriteLine("Kafka test started!");
        string path = @"D:\Git\10-Master\Experiments\Experiment1_Results\Kafka.txt";
        File.Delete(path);
        
        var index = 0;
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        void ProcessMessage(string key, string value)
        {
            stopwatch.Stop();
            var val = Guid.Parse(value);
            if (_last != val)
                throw new Exception();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            
            if (index > 10)
            {
                _results.Add(elapsedTime);
                //Console.WriteLine($"Kafka result in {elapsedTime} ms");
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(elapsedTime);
                }
            }

            if (index >= runs)
            {
                Console.WriteLine($"Kafka test done!");
                Console.WriteLine($"avg: {_results.Average()}, min: {_results.Min()}, max: {_results.Max()}");
                
                _cts.Cancel();
                return;
            }

            index += 1;
            _last = Guid.NewGuid();
            stopwatch.Restart();
            _p.Produce("input", "key", "" + _last);
        }

        _last = Guid.NewGuid();
        stopwatch.Restart();
        _p.Produce("input", "key", "" + _last);
        
        IConsumer.ProcessMessage action = ProcessMessage;
        try
        {
            await Task.Run(() => _c.Consume("output", action, _cts.Token), _cts.Token);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("The task was cancelled.");
        }
    }

    private static async Task RabbitRun(int runs)
    {
        Console.WriteLine("RabbitMQ test started!");
        string path = @"D:\Git\10-Master\Experiments\Experiment1_Results\Rabbit.txt";
        File.Delete(path);
        
        var index = 0;
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        void ProcessMessage(string key, string value)
        {
            stopwatch.Stop();
            var val = Guid.Parse(value);
            if (_last != val)
                throw new Exception();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            
            if (index > 10)
            {
                _results.Add(elapsedTime);
                //Console.WriteLine($"RabbitMQ result in {elapsedTime} ms");
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(elapsedTime);
                }
            }

            if (index >= runs)
            {
                Console.WriteLine($"RabbitMQ test done!");
                Console.WriteLine($"avg: {_results.Average()}, min: {_results.Min()}, max: {_results.Max()}");
                
                _cts.Cancel();
                return;
            }

            index += 1;
            _last = Guid.NewGuid();
            stopwatch.Restart();
            _rp.Produce("input", "key", "" + _last);
        }

        _last = Guid.NewGuid();
        stopwatch.Restart();
        _rp.Produce("input", "key", "" + _last);
        
        IConsumer.ProcessMessage action = ProcessMessage;
        try
        {
            await Task.Run(() => _rc.Consume("output", action, _cts.Token), _cts.Token);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("The task was cancelled.");
        }
    }
}