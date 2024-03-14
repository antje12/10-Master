using System.Diagnostics;
using ClassLibrary.Interfaces;
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
    private KafkaTopic InputTopic = KafkaTopic.Projectile;
    private KafkaTopic OutputTopic = KafkaTopic.Collision;

    private readonly KafkaAdministrator _admin;
    private readonly ProtoKafkaProducer<CollisionCheck> _producer;
    private readonly ProtoKafkaConsumer<Projectile> _consumer;

    //private readonly MongoDbBroker _mongoBroker;
    //private readonly RedisBroker _redisBroker;

    public bool IsRunning { get; private set; }

    public TickService()
    {
        Console.WriteLine($"TickService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _producer = new ProtoKafkaProducer<CollisionCheck>(config);
        _consumer = new ProtoKafkaConsumer<Projectile>(config);
        //_mongoBroker = new MongoDbBroker();
        //_redisBroker = new RedisBroker();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();
        IsRunning = true;
        Console.WriteLine($"TickService started");
        await _admin.CreateTopic(InputTopic);
        IProtoConsumer<Projectile>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(InputTopic, action, ct);
        IsRunning = false;
        Console.WriteLine($"TickService stopped");
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

        _producer.Produce(OutputTopic, output.EntityId.ToString(), output);
    }
}