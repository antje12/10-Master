using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Classes.Messages;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using WorldService.Interfaces;

namespace WorldService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class WorldService : BackgroundService, IConsumerService
{
    private const string GroupId = "world-group";
    private KafkaTopic InputTopic = KafkaTopic.World;
    private KafkaTopic OutputTopic = KafkaTopic.LocalState;

    private readonly KafkaAdministrator _admin;
    private readonly KafkaProducer<LocalState> _producer;
    private readonly KafkaConsumer<WorldChange> _consumer;

    private readonly MongoDbBroker _mongoBroker;

    public bool IsRunning { get; private set; }

    public WorldService()
    {
        Console.WriteLine($"WorldService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _producer = new KafkaProducer<LocalState>(config);
        _consumer = new KafkaConsumer<WorldChange>(config);
        _mongoBroker = new MongoDbBroker();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"WorldService started");

        await _admin.CreateTopic(InputTopic);
        IConsumer<WorldChange>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(InputTopic, action, ct);

        IsRunning = false;
        Console.WriteLine($"WorldService stopped");
    }

    private void ProcessMessage(string key, WorldChange value)
    {
        var avatar = new Avatar()
        {
            Id = value.PlayerId,
            Location = value.NewLocation
        };

        var output = new LocalState()
        {
            PlayerId = value.PlayerId,
            Avatars = new List<Avatar>()
        };

        _mongoBroker.UpsertAvatarLocation(avatar);

        var avatars = _mongoBroker.ReadScreen(new Coordinates()
        {
            X = avatar.Location.X,
            Y = avatar.Location.Y
        });

        foreach (var a in avatars)
        {
            var na = new Avatar()
            {
                Id = a.Id,
                Name = "test",
                Location = a.Location
            };
            output.Avatars.Add(na);
        }

        _producer.Produce($"{OutputTopic}_{output.PlayerId}", key, output);
    }
}