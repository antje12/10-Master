using System.Diagnostics;
using AIService.Interfaces;
using ClassLibrary.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;

namespace AIService.Services;

public class AIService : BackgroundService, IConsumerService
{
    private string _groupId = "ai-group";
    private KafkaTopic _inputTopic = KafkaTopic.Ai;
    private KafkaTopic _outputTopic = KafkaTopic.Input;

    internal IAdministrator Admin;
    internal IProtoProducer<Input_M> Producer;
    internal IProtoConsumer<Ai_M> Consumer;

    internal RedisBroker RedisBroker;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public AIService()
    {
        Console.WriteLine("AIService created");
        var config = new KafkaConfig(_groupId, localTest);
        Admin = new KafkaAdministrator(config);
        Producer = new KafkaProducer<Input_M>(config);
        Consumer = new KafkaConsumer<Ai_M>(config);
        RedisBroker = new RedisBroker();
    }

    internal async Task ExecuteAsync()
    {
        var cts = new CancellationTokenSource();
        await ExecuteAsync(cts.Token);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        RedisBroker.Connect();
        Console.WriteLine("AIService started");
        await Admin.CreateTopic(_inputTopic);
        IProtoConsumer<Ai_M>.ProcessMessage action = ProcessMessage;
        await Consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("AIService stopped");
    }

    internal void ProcessMessage(string key, Ai_M value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(Ai_M agent)
    {
        if (RedisBroker.Get(Guid.Parse(agent.Id)) == null)
            return;
        
        var entities = RedisBroker
            .GetEntities(agent.Location.X, agent.Location.Y)
            .Where(x => x.Id.ToString() != agent.Id).ToList();
        var targets = entities
            .OfType<Player>();

        var from = (long) agent.LastUpdate;
        var to = DateTime.UtcNow.Ticks;
        var difference = TimeSpan.FromTicks(to - from);
        var deltaTime = difference.TotalSeconds;
        
        var output = new Input_M
        {
            AgentId = agent.Id,
            AgentLocation = new Coordinates_M
            {
                X = agent.Location.X,
                Y = agent.Location.Y
            },
            GameTime = deltaTime,
            Source = Source.Ai,
            LastUpdate = to
        };

        var obstacles = entities
            .OfType<Enemy>()
            .Select(x => new Node((int) x.Location.X, (int) x.Location.Y)).ToList();
        var start = new Node((int) agent.Location.X, (int) agent.Location.Y);
        var target = targets.MinBy(t =>
            AStar.H((int) agent.Location.X, (int) agent.Location.Y, (int) t.Location.X, (int) t.Location.Y));

        var nextStep = target != null
            ? AStar.Search(start, new Node((int) target.Location.X, (int) target.Location.Y), obstacles)
            : AStar.SurvivalSearch(start, obstacles);

        if (target != null && 
            AStar.H((int) agent.Location.X, (int) agent.Location.Y, (int) target.Location.X, (int) target.Location.Y) < 150)
        {
            output.MouseLocation = new Coordinates_M
            {
                X = target.Location.X,
                Y = target.Location.Y
            };
            output.KeyInput.Add(GameKey.Attack);
        }
        else
        {
            if (nextStep.X < start.X)
                output.KeyInput.Add(GameKey.Left);
            if (start.X < nextStep.X)
                output.KeyInput.Add(GameKey.Right);
            if (nextStep.Y < start.Y)
                output.KeyInput.Add(GameKey.Up);
            if (start.Y < nextStep.Y)
                output.KeyInput.Add(GameKey.Down);
        }

        if (!output.KeyInput.Any())
        {
            Console.WriteLine("AI dead!");
        }
        
        Producer.Produce(_outputTopic, output.AgentId, output);
    }
}