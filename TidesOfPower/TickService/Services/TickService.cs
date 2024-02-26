using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Classes.Messages;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;
using TickService.Interfaces;

namespace TickService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class TickService : BackgroundService, IConsumerService
{
    private KafkaTopic OutputTopic = KafkaTopic.LocalState;
    
    private readonly KafkaProducer<LocalState> _producer;

    private readonly MongoDbBroker _mongoBroker;

    public bool IsRunning { get; private set; }

    public TickService()
    {
        Console.WriteLine($"TickService created");
        var config = new KafkaConfig("");
        _producer = new KafkaProducer<LocalState>(config);
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
            var avatars = _mongoBroker.ReadEntities();
            foreach (var avatar in avatars)
            {
                //SendState(avatar);
            }
            
            Thread.Sleep(33);
        }

        IsRunning = false;
        Console.WriteLine($"TickService stopped");
    }

    private void SendState(Avatar avatar)
    {
        var state = _mongoBroker.ReadScreen(avatar.Location);
        var output = new LocalState()
        {
            PlayerId = avatar.Id,
            Sync = SyncType.Full,
            Avatars = new List<Avatar>()
        };

        foreach (var a in state)
        {
            var na = new Avatar()
            {
                Id = a.Id,
                Location = a.Location
            };
            output.Avatars.Add(na);
        }

        _producer.Produce($"{OutputTopic}_{output.PlayerId}", output.PlayerId.ToString(), output);
    }
}