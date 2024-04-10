using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using InputService.Interfaces;
using ClassLibrary.Messages.Protobuf;

namespace InputService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class InputService : BackgroundService, IConsumerService
{
    private string _groupId = "input-group";
    private KafkaTopic _inputTopic = KafkaTopic.Input;
    private KafkaTopic _outputTopicC = KafkaTopic.Collision;
    private KafkaTopic _outputTopicW = KafkaTopic.World;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<Collision_M> _producerC;
    private ProtoKafkaProducer<World_M> _producerW;
    private ProtoKafkaConsumer<Input_M> _consumer;

    private Dictionary<string, DateTime> ClientAttacks = new();
    
    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public InputService()
    {
        Console.WriteLine("InputService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producerC = new ProtoKafkaProducer<Collision_M>(config);
        _producerW = new ProtoKafkaProducer<World_M>(config);
        _consumer = new ProtoKafkaConsumer<Input_M>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("InputService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<Input_M>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("InputService stopped");
    }

    private void ProcessMessage(string key, Input_M value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(key, value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(string key, Input_M value)
    {
        if (!string.IsNullOrEmpty(value.EventId))
        {
            string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Got {value.EventId} at {timestampWithMs}");
        }
        var oldKeys = ClientAttacks.Where(x => x.Value < DateTime.Now)
            .Select(x => x.Key);
        foreach (var oldKey in oldKeys)
        {
            ClientAttacks.Remove(oldKey);
        }
        
        if (value.KeyInput.Any(x => x is GameKey.Up or GameKey.Down or GameKey.Left or GameKey.Right))
            Move(key, value);
        if (value.KeyInput.Any(x => x is GameKey.Attack))
            Attack(key, value);
        if (value.KeyInput.Any(x => x is GameKey.Interact))
            Interact(key, value);
    }

    private void Move(string key, Input_M value)
    {
        ClassLibrary.GameLogic.Move.Avatar(value.AgentLocation.X, value.AgentLocation.Y, value.KeyInput.ToList(),
            value.GameTime,
            out float toX, out float toY);

        var msgOut = new Collision_M()
        {
            EntityId = value.AgentId,
            EntityType = value.Source == Source.Ai ? EntityType.Ai : EntityType.Player,
            
            FromLocation = value.AgentLocation,
            ToLocation = new()
            {
                X = toX,
                Y = toY
            },
            LastUpdate = value.LastUpdate,
            EventId = value.EventId
        };
        _producerC.Produce(_outputTopicC, key, msgOut);
        if (!string.IsNullOrEmpty(value.EventId))
        {
            string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Sent {value.EventId} at {timestampWithMs}");
        }
    }

    private void Attack(string key, Input_M value)
    {
        if (ClientAttacks.ContainsKey(key))
            return;
        
        ClientAttacks.Add(key, DateTime.Now.AddSeconds(1));
        
        var x = value.MouseLocation.X - value.AgentLocation.X;
        var y = value.MouseLocation.Y - value.AgentLocation.Y;
        var length = (float) Math.Sqrt(x * x + y * y);
        if (length > 0)
        {
            x /= length;
            y /= length;
        }

        var spawnX = value.AgentLocation.X + x * (25 + 5 + 1);
        var spawnY = value.AgentLocation.Y + y * (25 + 5 + 1);

        var msgOut = new World_M()
        {
            EntityId = Guid.NewGuid().ToString(),
            Change = Change.SpawnBullet,
            Location = new Coordinates_M() {X = spawnX, Y = spawnY},
            Direction = new Coordinates_M() {X = x, Y = y}
        };
        _producerW.Produce(_outputTopicW, key, msgOut);
    }

    private void Interact(string key, Input_M value)
    {
        var msgOut = new World_M()
        {
            EntityId = Guid.NewGuid().ToString(),
            Change = Change.SpawnAi,
            Location = value.MouseLocation,
        };
        _producerW.Produce(_outputTopicW, key, msgOut);
    }
}