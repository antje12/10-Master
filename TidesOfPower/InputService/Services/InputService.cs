using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Messages;
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
    private readonly KafkaProducer<CollisionCheck> _producer;
    private readonly KafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public InputService()
    {
        Console.WriteLine($"InputService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _admin.CreateTopic(KafkaTopic.Input);
        _producer = new KafkaProducer<CollisionCheck>(config);
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
        var output = new CollisionCheck()
        {
            PlayerId = value.PlayerId,
            FromLocation = value.Location,
            ToLocation = new Coordinates()
            {
                X = value.Location.X,
                Y = value.Location.Y
            }
        };

        var moving = false;
        var attacking = false;
        var interacting = false;

        foreach (var input in value.KeyInput)
        {
            switch (input)
            {
                case GameKey.Up:
                    output.ToLocation.Y -= 100 * (float) value.GameTime;
                    moving = true;
                    break;
                case GameKey.Down:
                    output.ToLocation.Y += 100 * (float) value.GameTime;
                    moving = true;
                    break;
                case GameKey.Left:
                    output.ToLocation.X -= 100 * (float) value.GameTime;
                    moving = true;
                    break;
                case GameKey.Right:
                    output.ToLocation.X += 100 * (float) value.GameTime;
                    moving = true;
                    break;
                case GameKey.Attack:
                    attacking = true;
                    break;
                case GameKey.Interact:
                    interacting = true;
                    break;
                default:
                    break;
            }
        }

        if (moving)
        {
            _producer.Produce(KafkaTopic.Collision, key, output);
        }

        if (attacking)
        {
        }

        if (interacting)
        {
        }
    }
}