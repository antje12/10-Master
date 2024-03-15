using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Avro;
using PhysicsService.Interfaces;

namespace PhysicsService.Services;

public class PhysicsService : BackgroundService, IConsumerService
{
    private string _groupId = "physics-group";
    private KafkaTopic _inputTopic = KafkaTopic.Physics;
    private KafkaTopic _outputTopic = KafkaTopic.Collision;

    private KafkaAdministrator _admin;
    private KafkaProducer<LocalState> _producer;
    private KafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public PhysicsService()
    {
        Console.WriteLine($"PhysicsService created");
        var config = new KafkaConfig(_groupId);
        _admin = new KafkaAdministrator(config);
        _producer = new KafkaProducer<LocalState>(config);
        _consumer = new KafkaConsumer<Input>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"PhysicsService started");

        await _admin.CreateTopic(_inputTopic);
        IConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);

        IsRunning = false;
        Console.WriteLine($"PhysicsService stopped");
    }

    private void ProcessMessage(string key, Input value)
    {
        var output = new LocalState()
        {
            PlayerId = value.PlayerId
        };

        _producer.Produce(_outputTopic, key, output);
    }
}