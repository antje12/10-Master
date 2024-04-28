using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using InputService.Interfaces;
using CollisionCheck = ClassLibrary.Messages.Avro.CollisionCheck;
using Input = ClassLibrary.Messages.Avro.Input;

namespace InputService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class InputService : BackgroundService, IConsumerService
{
    private string _groupId = "input-group";
    private KafkaTopic _inputTopic = KafkaTopic.Input;
    private KafkaTopic _outputTopicC = KafkaTopic.Collision;

    private KafkaAdministrator _admin;
    private AvroKafkaProducer<CollisionCheck> _avroProducer;
    private AvroKafkaConsumer<Input> _avroConsumer;

    private Dictionary<string, DateTime> ClientAttacks = new();
    
    public bool IsRunning { get; private set; }
    private bool localTest = false;

    public InputService()
    {
        Console.WriteLine("InputService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _avroProducer = new AvroKafkaProducer<CollisionCheck>(config);
        _avroConsumer = new AvroKafkaConsumer<Input>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("InputService started");
        await _admin.CreateTopic(_inputTopic);
        IAvroConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _avroConsumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("InputService stopped");
    }

    private void ProcessMessage(string key, Input value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(key, value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(string key, Input value)
    {
        var oldKeys = ClientAttacks.Where(x => x.Value < DateTime.Now)
            .Select(x => x.Key);
        foreach (var oldKey in oldKeys)
        {
            ClientAttacks.Remove(oldKey);
        }
        
        if (value.KeyInput.Any(x => x is GameKey.Up or GameKey.Down or GameKey.Left or GameKey.Right))
            Move(key, value);
    }

    private void Move(string key, Input value)
    {
        ClassLibrary.GameLogic.Move.Avatar(value.AgentLocation.X, value.AgentLocation.Y, value.KeyInput.ToList(),
            value.GameTime,
            out float toX, out float toY);

        var msgOut = new CollisionCheck()
        {
            EntityId = value.AgentId,
            FromLocation = value.AgentLocation,
            ToLocation = new()
            {
                X = toX,
                Y = toY
            },
            Timer = value.GameTime
        };
        _avroProducer.Produce(_outputTopicC, key, msgOut);
    }
}