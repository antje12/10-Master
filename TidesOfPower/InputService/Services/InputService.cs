using ClassLibrary.Classes.Client;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using InputService.Interfaces;

namespace InputService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class InputService : BackgroundService, IConsumerService
{
    private const string GroupId = "input-group";

    private readonly KafkaAdministrator _admin;
    private readonly KafkaProducer<Output> _producer;
    private readonly KafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public InputService()
    {
        Console.WriteLine($"InputService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _admin.CreateTopic(KafkaTopic.Input);
        _producer = new KafkaProducer<Output>(config);
        _consumer = new KafkaConsumer<Input>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"InputService started");

        await _admin.CreateTopic(KafkaTopic.Input);
        IConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(KafkaTopic.Input, action, ct);

        IsRunning = false;
        Console.WriteLine($"InputService stopped");
    }

    private void ProcessMessage(string key, Input value)
    {
        var output = new Output()
        {
            PlayerId = value.PlayerId,
            Location = value.Location
        };
        foreach (var input in value.KeyInput)
        {
            switch (input)
            {
                case GameKey.Up:
                    output.Location.Y -= 100 * (float) value.Timer;
                    break;
                case GameKey.Down:
                    output.Location.Y += 100 * (float) value.Timer;
                    break;
                case GameKey.Left:
                    output.Location.X -= 100 * (float) value.Timer;
                    break;
                case GameKey.Right:
                    output.Location.X += 100 * (float) value.Timer;
                    break;
                case GameKey.Attack:
                case GameKey.Interact:
                default:
                    break;
            }
        }

        //_producer.Produce($"{KafkaTopic.LocalState}_{output.PlayerId.ToString()}", key, output);
        _producer.Produce(KafkaTopic.LocalState.ToString(), key, output);
    }
}