using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using InputService.Interfaces;

namespace InputService.Controllers;

[ApiController]
[Route("[controller]")]
public class InputServiceController : ControllerBase
{
    //https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/AvroSpecific/Program.cs
    private const string _version = "1.00";
    
    private const string _kafkaServers = "localhost:19092";
    private const string _groupId = "msg-group";
    private const string _schemaRegistry = "localhost:8081";

    private static readonly SchemaRegistryConfig _schemaRegistryConfig;
    private static readonly AdminClientConfig _adminConfig;
    private static readonly ProducerConfig _producerConfig;
    private static readonly ConsumerConfig _consumerConfig;
    
    private static KafkaAdministrator _admin;
    private static KafkaProducer _producer;
    private static KafkaConsumer _consumer;
    
    private static CancellationTokenSource? _cancellationTokenSource;
    private readonly  IConsumerService _consumerService;
    private static Task? _task;
    
    private static int test = 0;

    public InputServiceController(IConsumerService consumerService)
    {
        Console.WriteLine($"InputServiceController");
        _consumerService = consumerService;
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = _schemaRegistry
        };
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = _kafkaServers
        };
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _kafkaServers,
            Acks = Acks.None,
            LingerMs = 0,
            BatchSize = 1
        };
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _admin = new KafkaAdministrator(adminConfig);
        _producer = new KafkaProducer(producerConfig, schemaRegistryConfig);
        _consumer = new KafkaConsumer(consumerConfig, schemaRegistryConfig);

        _admin.CreateTopic("topic");
    }
    
    [HttpGet("Version")]
    public object Version()
    {
        return $"Service version = {_version}";
    }

    [HttpGet("Test")]
    public object Test()
    {
        return $"Service test = {test}";
    }

    [HttpGet("Status")]
    public object Status()
    {
        return $"Service running = {!_cancellationTokenSource?.IsCancellationRequested ?? false}, alt = {_consumerService.IsRunning}";
    }

    [HttpGet("Start")]
    public object Start()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        IConsumer.ProcessMessage action = null;
        _task = _consumer.Consume("topic", action, _cancellationTokenSource);
        //var serviceTask = Task.Run(() =>
        //        ServiceLoop("topic", _cancellationTokenSource.Token),
        //    _cancellationTokenSource.Token);
        return "Service started";
    }

    [HttpGet("Stop")]
    public object Stop()
    {
        _cancellationTokenSource?.Cancel();
        return "Service stopped";
    }
}