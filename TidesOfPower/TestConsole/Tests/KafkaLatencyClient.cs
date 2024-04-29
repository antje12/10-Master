using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;

namespace TestConsole.Tests;

public class KafkaLatencyClient
{
    private string _groupId = "output-group";
    private KafkaTopic _outputTopic = KafkaTopic.Input;
    private KafkaTopic _inputTopic = KafkaTopic.LocalState;

    private KafkaConfig _config;
    private KafkaAdministrator _admin;
    private KafkaProducer<Input_M> _producer;
    private KafkaConsumer<LocalState_M> _consumer;

    private int _index;
    private string _path;
    private Stopwatch _timer;
    private List<long> _results;
    private CancellationTokenSource _cts;

    private int _counter;
    private int _testCount;
    private int _padding = 100;

    private Guid _testId;
    private string _testTopic;
    private Input_M _msg;

    public KafkaLatencyClient(int index, int testCount)
    {
        _testCount = testCount;
        _config = new KafkaConfig(_groupId, true);
        _admin = new KafkaAdministrator(_config);
        _producer = new KafkaProducer<Input_M>(_config);
        _consumer = new KafkaConsumer<LocalState_M>(_config);

        _index = index;
        _timer = new Stopwatch();
        _results = new List<long>();
        _cts = new CancellationTokenSource();

        _testId = Guid.NewGuid();
        _testTopic = $"{KafkaTopic.LocalState}_{_testId}";
        _msg = new Input_M()
        {
            AgentId = _testId.ToString(),
            AgentLocation = new Coordinates_M() {X = _index * 1000, Y = _index * 1000},
            GameTime = 0.0166667, // monogame = 60 updates a second
            EventId = Guid.NewGuid().ToString(),
            Source = Source.Player
        };
        _msg.KeyInput.Add(GameKey.Right);
    }

    public async Task Test()
    {
        _path = $@"D:\Git\10-Master\Experiments\Latency\Client{_index}_Latency.csv";
        File.Delete(_path);

        await _admin.CreateTopic(_testTopic);
        _cts = new CancellationTokenSource();

        _timer.Start();
        _producer.Produce(KafkaTopic.Input, _testId.ToString(), _msg);

        IProtoConsumer<LocalState_M>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_testTopic, action, _cts.Token);
    }

    void ProcessMessage(string key, LocalState_M value)
    {
        if (value.Sync != Sync.Full)
            return;

        _timer.Stop();
        var elapsedTime = _timer.ElapsedMilliseconds;
        Console.WriteLine($"Latency test {_index}: {elapsedTime} ms");
        var timestamp = DateTime.UtcNow;
        using (StreamWriter file = File.AppendText(_path))
        {
            file.WriteLine($"{timestamp:o};{elapsedTime}"); // Using "o" format for ISO 8601 format
        }

        //if (_counter > _padding)
        //{
        //    _results.Add(elapsedTime);
        //}
        //
        //if (_counter >= _testCount + _padding)
        //{
        //    Console.WriteLine(
        //        $"Client{_index} results {_results.Count}, avg {_results.Average()} ms, min {_results.Min()} ms, max {_results.Max()} ms");
        //    File.WriteAllLines($@"D:\Git\10-Master\Experiments\Experiment3_Results\Client{_index}_results.txt",
        //        _results.Select(r => r.ToString()));
        //    _cts.Cancel();
        //    return;
        //}

        _msg.AgentLocation = value.Agents
            .First(x => x.Id == _testId.ToString()).Location;

        //_counter += 1;
        _timer.Restart();
        _producer.Produce(KafkaTopic.Input, _testId.ToString(), _msg);
    }
}