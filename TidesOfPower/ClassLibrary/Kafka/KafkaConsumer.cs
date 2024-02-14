using Avro.Specific;
using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary.Kafka;

public class KafkaConsumer<T> : IConsumer<T> where T : ISpecificRecord
{
    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IConsumer<string, T> _consumer;

    public KafkaConsumer(
        ConsumerConfig consumerConfig,
        SchemaRegistryConfig schemaRegistryConfig)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        _consumer = new ConsumerBuilder<string, T>(consumerConfig)
            .SetValueDeserializer(new AvroDeserializer<T>(_schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
        //_consumer = new ConsumerBuilder<string, T>(consumerConfig).Build();
    }

    public Task Consume(string topic, IConsumer<T>.ProcessMessage action, CancellationToken ct)
    {
        _consumer.Subscribe(topic);
        while (!ct.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(ct);
            var result = consumeResult.Message;
            Console.WriteLine(
                $"{result.Key} = {result.Value.Get(0)} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            action(result.Key, result.Value);
        }

        _consumer.Close();
        return Task.CompletedTask;
    }
}