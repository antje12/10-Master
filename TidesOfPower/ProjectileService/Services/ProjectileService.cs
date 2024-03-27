using System.Diagnostics;
using ClassLibrary.GameLogic;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ProjectileService.Interfaces;

namespace ProjectileService.Services;

public class ProjectileService : BackgroundService, IConsumerService
{
    private string _groupId = "projectile-group";
    private KafkaTopic _inputTopic = KafkaTopic.Projectile;
    private KafkaTopic _outputTopic = KafkaTopic.Collision;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<CollisionCheck> _producer;
    private ProtoKafkaConsumer<Projectile> _consumer;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public ProjectileService()
    {
        Console.WriteLine("ProjectileService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producer = new ProtoKafkaProducer<CollisionCheck>(config);
        _consumer = new ProtoKafkaConsumer<Projectile>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("ProjectileService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<Projectile>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("ProjectileService stopped");
    }

    private void ProcessMessage(string key, Projectile value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(Projectile projectile)
    {
        var output = new CollisionCheck()
        {
            EntityId = projectile.Id,
            Entity = EntityType.Projectile,
            FromLocation = new Coordinates()
            {
                X = projectile.Location.X,
                Y = projectile.Location.Y
            },
            Timer = projectile.TimeToLive,
            Direction = projectile.Direction,
            GameTime = projectile.LastUpdate
        };

        var from = (long) output.GameTime;
        var to = DateTime.UtcNow.Ticks;
        var difference = TimeSpan.FromTicks(to - from);
        var deltaTime = difference.TotalSeconds;

        Move.Projectile(projectile.Location.X, projectile.Location.Y, projectile.Direction.X,
            projectile.Direction.Y, deltaTime,
            out var time, out var toX, out var toY);

        output.Timer -= time;
        output.ToLocation = new Coordinates()
        {
            X = toX,
            Y = toY
        };
        output.GameTime = to;
        _producer.Produce(_outputTopic, output.EntityId, output);
    }
}