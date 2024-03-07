using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using TickService.Interfaces;
using ClassLibrary.Messages.Protobuf;

namespace TickService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class TickService : BackgroundService, IConsumerService
{
    private KafkaTopic OutputTopic = KafkaTopic.Collision;

    private readonly ProtoKafkaProducer<CollisionCheck> _producer;

    private readonly MongoDbBroker _mongoBroker;

    public bool IsRunning { get; private set; }

    public TickService()
    {
        Console.WriteLine($"TickService created");
        var config = new KafkaConfig("");
        _producer = new ProtoKafkaProducer<CollisionCheck>(config);
        _mongoBroker = new MongoDbBroker();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"TickService started");

        while (!ct.IsCancellationRequested)
        {
            var projectiles = _mongoBroker.GetEntities().OfType<ClassLibrary.Classes.Domain.Projectile>().ToList();
            foreach (var projectile in projectiles)
            {
                SendState(projectile);
            }

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