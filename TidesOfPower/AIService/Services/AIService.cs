using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using AIService.Interfaces;
using ClassLibrary.Messages.Avro;

namespace AIService.Services;

public class AIService : BackgroundService, IConsumerService
{
    private string _groupId = "ai-group";
    private KafkaTopic _inputTopic = KafkaTopic.AI;
    private KafkaTopic _outputTopic = KafkaTopic.Input;

    private KafkaAdministrator _admin;
    private KafkaProducer<LocalState> _producer;
    private KafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public AIService()
    {
        Console.WriteLine($"AIService created");
        var config = new KafkaConfig(_groupId);
        _admin = new KafkaAdministrator(config);
        _producer = new KafkaProducer<LocalState>(config);
        _consumer = new KafkaConsumer<Input>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"AIService started");

        await _admin.CreateTopic(_inputTopic);
        IConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(_inputTopic, action, ct);

        IsRunning = false;
        Console.WriteLine($"AIService stopped");
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