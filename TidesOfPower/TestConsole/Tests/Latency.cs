using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;

namespace TestConsole.Tests;

public class Latency
{
    private string _groupId = "output-group";
    private KafkaTopic _outputTopic = KafkaTopic.Input;
    private KafkaTopic _inputTopic = KafkaTopic.LocalState;
    private KafkaConfig _config;
    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<Input> _producer;
    private ProtoKafkaConsumer<LocalState> _consumer;

    public Latency()
    {
        _config = new KafkaConfig(_groupId, true);
        _admin = new KafkaAdministrator(_config);
        _producer = new ProtoKafkaProducer<Input>(_config);
        _consumer = new ProtoKafkaConsumer<LocalState>(_config);
    }

    public async Task Test()
    {
        var tasks = new List<Task>();
        var numberOfClients = 2;

        for (int i = 0; i < numberOfClients; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() => RunLatencyTest(index)));
        }

        await Task.WhenAll(tasks);
    }

    private async Task RunLatencyTest(int index)
    {
        Console.WriteLine($"Thread {index} running");
        
        var clientId = $"Client{index}";
        var testId = Guid.NewGuid();
        var testTopic = $"{KafkaTopic.LocalState}_{testId}";       
        Console.WriteLine($"Thread {index} creating topic {testTopic}");
        await _admin.CreateTopic(testTopic);

        var cts = new CancellationTokenSource();

        var count = 0;
        var testCount = 100;
        var results = new List<long>();

        var msg = new Input()
        {
            PlayerId = testId.ToString(),
            PlayerLocation = new Coordinates() {X = index*1000, Y = index*1000},
            GameTime = 0.5,
            EventId = Guid.NewGuid().ToString(),
            Source = Source.Player
        };
        msg.KeyInput.Add(GameKey.Right);
        
        Console.WriteLine($"Thread {index} player {msg.PlayerId} at {msg.PlayerLocation.X}:{msg.PlayerLocation.Y}");

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        void ProcessMessage(string key, LocalState value)
        {
            if (value.Sync != SyncType.Full)
                return;
            
            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Thread {index} Kafka result in {elapsedTime} ms");

            if (count > 0)
            {
                results.Add(elapsedTime);
            }

            if (count >= testCount)
            {
                Console.WriteLine(
                    $"{clientId} results {results.Count}, avg {results.Average()} ms, min {results.Min()} ms, max {results.Max()} ms");
                File.WriteAllLines($"{clientId}_results.txt", results.Select(r => r.ToString()));
                cts.Cancel();
                return;
            }

            msg.PlayerLocation = value.Avatars
                .First(x => x.Id == testId.ToString()).Location;

            count += 1;
            Console.WriteLine($"Thread {index} producing to topic {testTopic}");
            stopwatch.Restart();
            _producer.Produce(KafkaTopic.Input, testId.ToString(), msg);
        }

        stopwatch.Restart();
        _producer.Produce(KafkaTopic.Input, testId.ToString(), msg);

        IProtoConsumer<LocalState>.ProcessMessage action = ProcessMessage;
        Console.WriteLine($"Thread {index} consuming topic {testTopic}");
        try
        {
            await _consumer.Consume(testTopic, action, cts.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Thread {index} consuming topic {testTopic}");
            Console.WriteLine(e);
            throw;
        }
    }
}