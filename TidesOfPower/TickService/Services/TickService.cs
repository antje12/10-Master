using System.Diagnostics;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using TickService.Interfaces;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;

namespace TickService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class TickService : BackgroundService, IConsumerService
{
    private const string GroupId = "tick-group";
    private KafkaTopic OutputTopic = KafkaTopic.Collision;

    private readonly ProtoKafkaProducer<CollisionCheck> _producer;

    private readonly MongoDbBroker _mongoBroker;
    private readonly RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }

    public TickService()
    {
        Console.WriteLine($"TickService created");
        var config = new KafkaConfig(GroupId);
        _producer = new ProtoKafkaProducer<CollisionCheck>(config);
        _mongoBroker = new MongoDbBroker();
        _redisBroker = new RedisBroker();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"TickService started");

        var stopwatch = new Stopwatch();
        var s2 = new Stopwatch();
        
        while (!ct.IsCancellationRequested)
        {
            stopwatch.Restart();
            s2.Restart();
            var projectiles = _redisBroker.GetEntities().OfType<ClassLibrary.Classes.Domain.Projectile>().ToList();
            s2.Stop();
            
            foreach (var projectile in projectiles)
            {
                SendState(projectile);
            }
        
            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            if (elapsedTime > 20) Console.WriteLine($"Message processed in {elapsedTime} ms with {s2.ElapsedMilliseconds} ms DB time");

            Thread.Sleep(50);
        }

        IsRunning = false;
        Console.WriteLine($"TickService stopped");
    }

    private void SendState(ClassLibrary.Classes.Domain.Projectile projectile)
    {
        var output = new CollisionCheck()
        {
            EntityId = projectile.Id.ToString(),
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
            Timer = projectile.Timer - 1
        };

        var speed = 100;
        var deltaTime = 0.05f;

        output.ToLocation.X += projectile.Direction.X * speed * deltaTime;
        output.ToLocation.Y += projectile.Direction.Y * speed * deltaTime;

        _producer.Produce(OutputTopic, output.EntityId.ToString(), output);
    }
}