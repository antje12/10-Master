using System.Diagnostics;
using ClassLibrary.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.MongoDB;
using ClassLibrary.Redis;
using WorldService.Interfaces;

namespace WorldService.Services;

public class WorldService : BackgroundService, IConsumerService
{
    private string _groupId = "world-group";
    private KafkaTopic _inputTopic = KafkaTopic.World;
    private KafkaTopic _outputTopicLS = KafkaTopic.LocalState;
    private KafkaTopic _outputTopicP = KafkaTopic.Projectile;
    private KafkaTopic _outputTopicA = KafkaTopic.Ai;

    internal IAdministrator Admin;
    internal IProtoProducer<LocalState_M> ProducerLS;
    internal IProtoProducer<Projectile_M> ProducerP;
    internal IProtoProducer<Ai_M> ProducerA;
    internal IProtoConsumer<World_M> Consumer;

    //internal MongoDbBroker MongoBroker;
    //internal RedisBroker RedisBroker;

    private CancellationTokenSource _cts = new();
    public bool IsRunning { get; private set; }
    private bool localTest = false;

    public WorldService()
    {
        Console.WriteLine("WorldService created");
        var config = new KafkaConfig(_groupId, localTest);
        Admin = new KafkaAdministrator(config);
        ProducerLS = new KafkaProducer<LocalState_M>(config);
        ProducerP = new KafkaProducer<Projectile_M>(config);
        ProducerA = new KafkaProducer<Ai_M>(config);
        Consumer = new KafkaConsumer<World_M>(config);
        //MongoBroker = new MongoDbBroker();
        //RedisBroker = new RedisBroker();
    }

    public void StopService()
    {
        _cts.Cancel();
    }

    internal async Task ExecuteAsync()
    {
        await ExecuteAsync(_cts.Token);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        //RedisBroker.Connect(localTest);
        //MongoBroker.Connect(localTest);
        Console.WriteLine("WorldService started");
        await Admin.CreateTopic(_inputTopic);
        IProtoConsumer<World_M>.ProcessMessage action = ProcessMessage;
        var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);
        await Consumer.Consume(_inputTopic, action, linkedSource.Token);
        IsRunning = false;
        Console.WriteLine("WorldService stopped");
    }

    internal void ProcessMessage(string key, World_M value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(key, value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(string key, World_M value)
    {
        if (!string.IsNullOrEmpty(value.EventId))
        {
            string timestampWithMs = DateTime.UtcNow.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Got {value.EventId} at {timestampWithMs}");
        }

        switch (value.Change)
        {
            case Change.MovePlayer:
                MovePlayer(key, value);
                break;
        }
    }

    private void MovePlayer(string key, World_M value)
    {
        //var db = RedisBroker.Get(Guid.Parse(value.EntityId));

        var agent = new Agent_M
        {
            Id = value.EntityId,
            Location = value.Location,
            Name = "Player",
            WalkingSpeed = 0,
            LifePool = 0,
            Score = 0
        };
        agent.Score += value.Value;
        FullSync(key, value, agent);
    }

    private void FullSync(string key, World_M value, Agent_M agent)
    {
        var msgOut = new LocalState_M
        {
            Sync = Sync.Full,
            EventId = value.EventId
        };
        msgOut.Agents.Add(agent);
        ProducerLS.Produce($"{_outputTopicLS}_{value.EntityId}", key, msgOut);

        if (!string.IsNullOrEmpty(value.EventId))
        {
            string timestampWithMs = DateTime.UtcNow.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
            Console.WriteLine($"Sent {value.EventId} at {timestampWithMs}");
        }
    }

    private void DeltaSync(
        List<Player> players,
        List<Agent_M> agents,
        List<Projectile_M> projectiles,
        List<Treasure_M> treasures,
        Sync sync)
    {
        var msgOut = new LocalState_M
        {
            Sync = sync
        };
        msgOut.Agents.AddRange(agents);
        msgOut.Projectiles.AddRange(projectiles);
        msgOut.Treasures.AddRange(treasures);
        foreach (var player in players)
        {
            ProducerLS.Produce($"{_outputTopicLS}_{player.Id}", player.Id.ToString(), msgOut);
        }
    }
}