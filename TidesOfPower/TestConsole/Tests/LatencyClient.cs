using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;

namespace TestConsole.Tests;

public class LatencyClient
{
    private string _groupId = "output-group";
    private KafkaTopic _outputTopic = KafkaTopic.Input;
    private KafkaTopic _inputTopic = KafkaTopic.LocalState;

    private KafkaConfig _config;
    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<Input> _producer;
    private ProtoKafkaConsumer<LocalState> _consumer;

    private int _index;
    private Stopwatch _sw;
    private List<long> _results;
    private CancellationTokenSource _cts;

    private int _counter = 0;
    private int _testCount = 110;

    private Guid _testId;
    private string _testTopic;

    private Input _msg;

    public LatencyClient(int index)
    {
        _config = new KafkaConfig(_groupId, true);
        _admin = new KafkaAdministrator(_config);
        _producer = new ProtoKafkaProducer<Input>(_config);
        _consumer = new ProtoKafkaConsumer<LocalState>(_config);

        _index = index;
        _sw = new Stopwatch();
        _results = new List<long>();
        _cts = new CancellationTokenSource();

        _testId = Guid.NewGuid();
        _testTopic = $"{KafkaTopic.LocalState}_{_testId}";

        _msg = new Input()
        {
            PlayerId = _testId.ToString(),
            PlayerLocation = new Coordinates() {X = _index * 1000, Y = _index * 1000},
            GameTime =  0.0166667, // monogame = 60 updates a second
            EventId = Guid.NewGuid().ToString(),
            Source = Source.Player
        };
        _msg.KeyInput.Add(GameKey.Right);
    }

    public async Task RunLatencyTest()
    {
        await _admin.CreateTopic(_testTopic);
        _cts = new CancellationTokenSource();

        _sw.Start();
        _producer.Produce(KafkaTopic.Input, _testId.ToString(), _msg);

        IProtoConsumer<LocalState>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_testTopic, action, _cts.Token);
    }

    void ProcessMessage(string key, LocalState value)
    {
        if (value.Sync != SyncType.Full)
            return;

        _sw.Stop();
        var elapsedTime = _sw.ElapsedMilliseconds;
        //Console.WriteLine($"Thread {_index} Kafka result in {elapsedTime} ms");
        
        if (_counter > 10)
        {
            _results.Add(elapsedTime);
        }

        if (_counter >= _testCount)
        {
            Console.WriteLine(
                $"Client{_index} results {_results.Count}, avg {_results.Average()} ms, min {_results.Min()} ms, max {_results.Max()} ms");
            File.WriteAllLines(
                $"Client{_index}_results.txt",
                _results.Select(r => r.ToString()));
            _cts.Cancel();
            return;
        }

        _msg.PlayerLocation = value.Avatars
            .First(x => x.Id == _testId.ToString()).Location;

        _counter += 1;
        _sw.Restart();
        _producer.Produce(KafkaTopic.Input, _testId.ToString(), _msg);
    }
}