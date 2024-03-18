using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using AIService.Interfaces;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;

namespace AIService.Services;

public class AIService : BackgroundService, IConsumerService
{
    private string _groupId = "ai-group";
    private KafkaTopic _inputTopic = KafkaTopic.Ai;
    private KafkaTopic _outputTopic = KafkaTopic.Input;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<Input> _producer;
    private ProtoKafkaConsumer<AiAgent> _consumer;
    
    private RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public AIService()
    {
        Console.WriteLine($"AIService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producer = new ProtoKafkaProducer<Input>(config);
        _consumer = new ProtoKafkaConsumer<AiAgent>(config);
        _redisBroker = new RedisBroker(localTest);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine($"AIService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<AiAgent>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine($"AIService stopped");
    }

    private void ProcessMessage(string key, AiAgent value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        SendState(value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void SendState(AiAgent agent)
    {
        var targets = _redisBroker.GetEntities(agent.Location.X, agent.Location.Y)
            .OfType<ClassLibrary.Classes.Domain.Avatar>()
            .Where(x => x.Id.ToString() != agent.Id);
        
        var from = agent.LastUpdate.ToDateTime();
        var to = DateTime.UtcNow;
        TimeSpan difference = to - from;
        var deltaTime = difference.TotalSeconds;
        
        var output = new Input()
        {
            PlayerId = agent.Id.ToString(),
            PlayerLocation = new Coordinates()
            {
                X = agent.Location.X,
                Y = agent.Location.Y
            },
            GameTime = deltaTime,
            EventId = Guid.NewGuid().ToString(),
            Source = Source.Ai
        };

        var target = targets.FirstOrDefault();
        if (target != null)
        {
            output.KeyInput.Add(GameKey.Right);
        
            _producer.Produce(_outputTopic, output.PlayerId.ToString(), output);
        }
    }
}