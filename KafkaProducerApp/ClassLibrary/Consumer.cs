using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary;

public class Consumer
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly SchemaRegistryConfig _schemaRegistryConfig;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IConsumer<string, PlayerPos> _consumer;
    
    public Consumer(
        ConsumerConfig consumerConfig,
        SchemaRegistryConfig schemaRegistryConfig,
        CancellationTokenSource cancellationTokenSource)
    {
        _consumerConfig = consumerConfig;
        _schemaRegistryConfig = schemaRegistryConfig;
        _cancellationTokenSource = cancellationTokenSource;
        
        _schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
        _consumer = new ConsumerBuilder<string, PlayerPos>(_consumerConfig)
            .SetValueDeserializer(new AvroDeserializer<PlayerPos>(_schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
    }

    public Task StartConsumer(string topic)
    {
        return Task.Run(() => ConsumeLoop(topic), _cancellationTokenSource.Token);
    }

    private Task ConsumeLoop(string topicName)
    {
        _consumer.Subscribe(topicName);
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(_cancellationTokenSource.Token);
            var result = consumeResult.Message.Value;
            Console.Clear();
            Console.Write($"{result.ID}: {result.X},{result.Y} - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        }

        _consumer.Close();
        return Task.CompletedTask;
    }

    public void StopConsumer()
    {
        _cancellationTokenSource.Cancel();
        //_consumeTask.Wait(_cancellationTokenSource.Token); // Wait for the background task to complete
        //_cancellationTokenSource.Dispose();
    }
}