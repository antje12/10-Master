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

    internal IAdministrator Admin;
    internal IProtoProducer<Collision_M> Producer;
    internal IProtoConsumer<Projectile_M> Consumer;

    public bool IsRunning { get; private set; }
    private bool localTest = true;

    public ProjectileService()
    {
        Console.WriteLine("ProjectileService created");
        var config = new KafkaConfig(_groupId, localTest);
        Admin = new KafkaAdministrator(config);
        Producer = new KafkaProducer<Collision_M>(config);
        Consumer = new KafkaConsumer<Projectile_M>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine("ProjectileService started");
        await Admin.CreateTopic(_inputTopic);
        IProtoConsumer<Projectile_M>.ProcessMessage action = ProcessMessage;
        await Consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine("ProjectileService stopped");
    }

    internal async Task ExecuteAsync()
    {
        var cts = new CancellationTokenSource();
        await ExecuteAsync(cts.Token);
    }

    internal void ProcessMessage(string key, Projectile_M value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Process(value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Message processed in {elapsedTime} ms");
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
        Producer.Produce(_outputTopic, output.EntityId, output);
    }
}