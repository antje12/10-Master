using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using PhysicsService.Interfaces;

namespace PhysicsService.Services;

public class PhysicsService : BackgroundService, IConsumerService
{
    private string _groupId = "physics-group";
    private KafkaTopic _inputTopic = KafkaTopic.Physics;
    private KafkaTopic _outputTopic = KafkaTopic.Collision;

    private KafkaAdministrator _admin;
    private ProtoKafkaProducer<LocalState> _producer;
    private ProtoKafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public PhysicsService()
    {
        Console.WriteLine($"PhysicsService created");
        var config = new KafkaConfig(_groupId);
        _admin = new KafkaAdministrator(config);
        _producer = new ProtoKafkaProducer<LocalState>(config);
        _consumer = new ProtoKafkaConsumer<Input>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"PhysicsService started");

        await _admin.CreateTopic(_inputTopic);
        IProtoConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);

        IsRunning = false;
        Console.WriteLine($"PhysicsService stopped");
    }

    private void ProcessMessage(string key, Input value)
    {
        var output = new LocalState()
        {
            AgentId = value.AgentId
        };

        _producer.Produce(_outputTopic, key, output);
    }
}