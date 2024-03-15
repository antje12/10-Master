using System.Diagnostics;
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

    public ProjectileService()
    {
        Console.WriteLine($"ProjectileService created");
        var config = new KafkaConfig(_groupId);
        _admin = new KafkaAdministrator(config);
        _producer = new ProtoKafkaProducer<CollisionCheck>(config);
        _consumer = new ProtoKafkaConsumer<Projectile>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine($"ProjectileService started");
        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<Projectile>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine($"ProjectileService stopped");
    }

    private void ProcessMessage(string key, Projectile value)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        SendState(value);
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        if (elapsedTime > 20) Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void SendState(Projectile projectile)
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
            ToLocation = new Coordinates()
            {
                X = projectile.Location.X,
                Y = projectile.Location.Y
            },
            Timer = projectile.Timer - 1,
            Direction = projectile.Direction,
            GameTime = projectile.GameTime
        };
        
        var from = output.GameTime.ToDateTime();
        var to = DateTime.UtcNow;
        TimeSpan difference = to - from;
        
        var deltaTime = difference.TotalSeconds;
        var speed = 200;

        output.ToLocation.X += projectile.Direction.X * speed * (float) deltaTime;
        output.ToLocation.Y += projectile.Direction.Y * speed * (float) deltaTime;

        _producer.Produce(_outputTopic, output.EntityId.ToString(), output);
    }
}