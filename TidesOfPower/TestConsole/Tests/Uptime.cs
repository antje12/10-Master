using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;

namespace TestConsole.Tests;

public class Uptime
{
    private string _groupId = "output-group";
    private KafkaTopic _outputTopic = KafkaTopic.Input;
    private KafkaTopic _inputTopic = KafkaTopic.LocalState;

    private KafkaConfig _config;
    private KafkaAdministrator _admin;
    private KafkaProducer<Input_M> _producer;
    private KafkaConsumer<LocalState_M> _consumer;

    private string _path;
    private Stopwatch _timer;
    private CancellationTokenSource _cts;

    private Guid _testId;
    private string _testTopic;
    private Input_M _msg;

    public Uptime()
    {
        _config = new KafkaConfig(_groupId, true);
        _admin = new KafkaAdministrator(_config);
        _producer = new KafkaProducer<Input_M>(_config);
        _consumer = new KafkaConsumer<LocalState_M>(_config);

        _timer = new Stopwatch();
        _cts = new CancellationTokenSource();

        _testId = Guid.NewGuid();
        _testTopic = $"{KafkaTopic.LocalState}_{_testId}";
        _msg = new Input_M()
        {
            AgentId = _testId.ToString(),
            AgentLocation = new Coordinates_M() {X = 0, Y = 0},
            GameTime = 0.0166667, // monogame = 60 updates a second
            EventId = Guid.NewGuid().ToString(),
            Source = Source.Player
        };
        _msg.KeyInput.Add(GameKey.Right);
    }

    public async Task Test()
    {
        _path = @"D:\Git\10-Master\Experiments\Deployability.csv";
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
        Console.WriteLine($"Latency test: {elapsedTime} ms");
        var timestamp = DateTime.UtcNow;
        using (StreamWriter file = File.AppendText(_path))
        {
            file.WriteLine($"{timestamp:o};{elapsedTime}"); // Using "o" format for ISO 8601 format
        }

        _msg.AgentLocation = value.Agents
            .First(x => x.Id == _testId.ToString()).Location;

        _timer.Restart();
        _producer.Produce(KafkaTopic.Input, _testId.ToString(), _msg);
    }
}