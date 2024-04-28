using Avro.Specific;
using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary.Kafka;

public class AvroKafkaConsumer<T> : IConsumer<T> where T : ISpecificRecord
{
    private CachedSchemaRegistryClient _schemaRegistry;
    private IConsumer<string, T> _consumer;

    public AvroKafkaConsumer(KafkaConfig config)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(config.SchemaRegistryConfig);
        _consumer = new ConsumerBuilder<string, T>(config.ConsumerConfig)
            .SetValueDeserializer(new AvroDeserializer<T>(_schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error consuming topic: {e.Reason}"))
            .Build();
        //_consumer = new ConsumerBuilder<string, T>(consumerConfig).Build();
    }

    public async Task Consume(KafkaTopic topic, IConsumer<T>.ProcessMessage action, CancellationToken ct)
    {
        await Consume(topic.ToString(), action, ct);
    }

    public Task Consume(string topic, IConsumer<T>.ProcessMessage action, CancellationToken ct)
    {
        _consumer.Subscribe(topic);
        //Console.WriteLine("Consumption started");
        while (!ct.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(ct);
            var result = consumeResult.Message;
            //Console.WriteLine(
            //    $"{result.Key} = {result.Value.Get(0)} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            action(result.Key, result.Value);
        }

        _consumer.Close();
        return Task.CompletedTask;
    }
}