using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary;

public class KafkaConsumer
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly SchemaRegistryConfig _schemaRegistryConfig;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IConsumer<string, string> _consumer;

    public delegate void OnMessage (string key, string message);

    public KafkaConsumer(
        ConsumerConfig consumerConfig,
        SchemaRegistryConfig schemaRegistryConfig,
        CancellationTokenSource cancellationTokenSource)
    {
        _consumerConfig = consumerConfig;
        _schemaRegistryConfig = schemaRegistryConfig;
        _cancellationTokenSource = cancellationTokenSource;

        _schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
        //_consumer = new ConsumerBuilder<string, PlayerPos>(_consumerConfig)
        //    .SetValueDeserializer(new AvroDeserializer<PlayerPos>(_schemaRegistry).AsSyncOverAsync())
        //    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
        //    .Build();
        _consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
    }

    public Task StartConsumer(string topic, OnMessage action)
    {
        return Task.Run(() => ConsumeLoop(topic, action), _cancellationTokenSource.Token);
    }

    private Task ConsumeLoop(string topic, OnMessage onMessage)
    {
        _consumer.Subscribe(topic);
        while (true)
        {
            var consumeResult = _consumer.Consume();
            var result = consumeResult.Message;
            Console.WriteLine(
                $"{result.Key} = {result.Value} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            onMessage(result.Key, result.Value);
            //consumer.Close(); 
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