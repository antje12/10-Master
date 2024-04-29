using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Google.Protobuf;

namespace ClassLibrary.Kafka;

public class KafkaConsumer<T> : IProtoConsumer<T> where T : class, IMessage<T>, new()
{
    private CachedSchemaRegistryClient _schemaRegistry;
    private IConsumer<string, T> _consumer;

    public KafkaConsumer(KafkaConfig config)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(config.SchemaRegistryConfig);
        _consumer = new ConsumerBuilder<string, T>(config.ConsumerConfig)
            .SetValueDeserializer(new ProtobufDeserializer<T>().AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error consuming topic: {e.Reason}"))
            .Build();
    }

    public async Task Consume(KafkaTopic topic, IProtoConsumer<T>.ProcessMessage action, CancellationToken ct)
    {
        await Consume(topic.ToString(), action, ct);
    }

    public Task Consume(string topic, IProtoConsumer<T>.ProcessMessage action, CancellationToken ct)
    {
        try
        {
            _consumer.Subscribe(topic);
            while (!ct.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(ct);
                var result = consumeResult.Message;
                action(result.Key, result.Value);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Consumption cancelled");
        }
        finally
        {
            _consumer.Close();
        }

        return Task.CompletedTask;
    }
}