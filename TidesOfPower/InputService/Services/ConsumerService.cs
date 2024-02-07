using Confluent.Kafka;
using InputService.Interfaces;

namespace InputService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class ConsumerService : BackgroundService, IConsumerService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private bool _isRunning;
    public bool IsRunning => _isRunning;
    
    public ConsumerService()
    {
        Console.WriteLine($"ConsumerService");
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:19092",
            GroupId = "msg-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();
        _isRunning = true;
        Console.WriteLine($"ExecuteAsync");
        _consumer.Subscribe("InventoryUpdates");

        while (!stoppingToken.IsCancellationRequested)
        {
            ProcessKafkaMessage(stoppingToken);

            Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        Console.WriteLine($"Done");
        _consumer.Close();
        _isRunning = false;
    }

    private void ProcessKafkaMessage(CancellationToken stoppingToken)
    {
        try
        {
            var consumeResult = _consumer.Consume(stoppingToken);

            var message = consumeResult.Message.Value;

            Console.WriteLine($"Received inventory update: {message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing Kafka message: {ex.Message}");
        }
    }
}