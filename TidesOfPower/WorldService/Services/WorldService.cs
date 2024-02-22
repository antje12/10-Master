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
        _admin.CreateTopic(KafkaTopic.World);
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

        await _admin.CreateTopic(KafkaTopic.World);
        IConsumer<WorldChange>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(KafkaTopic.World, action, ct);

        IsRunning = false;
        Console.WriteLine($"WorldService stopped");
    }

    private void ProcessMessage(string key, WorldChange value)
    {
        var avatar = new Avatar()
        {
            Id = value.PlayerId,
            Name = "test",
            Location = value.NewLocation
        };
        
        var output = new LocalState()
        {
            PlayerId = value.PlayerId,
            Location = value.NewLocation,
            Avatars = new List<Avatar>()
        };
        
        _mongoBroker.UpsertAvatarLocation(avatar);
        
        //var avatar = _mongoBroker.ReadAvatar(value.PlayerId);
        //if (avatar != null)
        //{
        //    Console.WriteLine("Avatar found");
        //    avatar.Location = output.Location;
        //    _mongoBroker.UpdateAvatarLocation(avatar);
        //}
        //else
        //{
        //    Console.WriteLine("No avatar found");
        //    _mongoBroker.CreateAvatar(new Avatar()
        //    {
        //        Id = output.PlayerId,
        //        Name = "test",
        //        Location = output.Location
        //    });
        //}
        
        var avatars = _mongoBroker.ReadScreen(new Coordinates() 
        { 
            X = output.Location.X, 
            Y = output.Location.Y 
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
        
        //_producer.Produce($"{KafkaTopic.LocalState}_{output.PlayerId.ToString()}", key, output);
        _producer.Produce(KafkaTopic.LocalState.ToString(), key, output);
    }
}