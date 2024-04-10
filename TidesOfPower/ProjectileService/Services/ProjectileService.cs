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
    private ProtoKafkaProducer<Collision_M> _producer;
    private ProtoKafkaConsumer<Projectile_M> _consumer;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public ProjectileService()
    {
        Console.WriteLine("ProjectileService created");
        var config = new KafkaConfig(_groupId, localTest);
        _admin = new KafkaAdministrator(config);
        _producer = new ProtoKafkaProducer<Collision_M>(config);
        _consumer = new ProtoKafkaConsumer<Projectile_M>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("ProjectileService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<Projectile_M>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("ProjectileService stopped");
    }

    private void ProcessMessage(string key, Projectile_M value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Process(Projectile_M projectile)
    {
        var output = new Collision_M()
        {
            EntityId = projectile.Id,
            EntityType = EntityType.Bullet,
            FromLocation = new Coordinates_M()
            {
                X = projectile.Location.X,
                Y = projectile.Location.Y
            },
            TTL = projectile.TTL,
            Direction = projectile.Direction,
            LastUpdate = projectile.LastUpdate
        };

        var from = (long) output.LastUpdate;
        var to = DateTime.UtcNow.Ticks;
        var difference = TimeSpan.FromTicks(to - from);
        var deltaTime = difference.TotalSeconds;

        Move.Projectile(projectile.Location.X, projectile.Location.Y, projectile.Direction.X,
            projectile.Direction.Y, deltaTime,
            out var time, out var toX, out var toY);

        output.TTL -= time;
        output.ToLocation = new Coordinates_M()
        {
            X = toX,
            Y = toY
        };
        output.LastUpdate = to;
        _producer.Produce(_outputTopic, output.EntityId, output);
    }
}