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
        switch (value.Change)
        {
            case ChangeType.MovePlayer:
                MovePlayer(key, value);
                break;
            case ChangeType.SpawnBullet:
                SpawnBullet(key, value);
                break;
        }
    }

    private void MovePlayer(string key, WorldChange value)
    {
        var player = new Avatar()
        {
            Id = value.EntityId,
            Location = value.Location
        };

        _mongoBroker.UpsertAvatarLocation(player);
        var avatars = _mongoBroker.ReadScreen(player.Location);

        var output = new LocalState()
        {
            PlayerId = player.Id,
            Sync = SyncType.Full,
            Avatars = avatars.Select(a => new Avatar()
            {
                Id = a.Id,
                Location = a.Location
            }).ToList()
        };
        _producer.Produce($"{OutputTopic}_{player.Id}", key, output);

        var enemies = output.Avatars.Where(x => x.Id != output.PlayerId);
        foreach (var enemy in enemies)
        {
            var deltaOutput = new LocalState()
            {
                PlayerId = enemy.Id,
                Sync = SyncType.Delta,
                Avatars = new List<Avatar>() {player}
            };
            _producer.Produce($"{OutputTopic}_{enemy.Id}", enemy.Id.ToString(), deltaOutput);
        }
    }

    private void SpawnBullet(string key, WorldChange value)
    {
        //var avatars = _mongoBroker.ReadScreen(value.Location);
    }
}