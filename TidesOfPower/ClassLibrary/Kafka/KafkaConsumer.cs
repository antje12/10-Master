using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace ClassLibrary.Kafka;

public class KafkaConsumer : IConsumer
{
    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumer(
        ConsumerConfig consumerConfig,
        SchemaRegistryConfig schemaRegistryConfig)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        //_consumer = new ConsumerBuilder<string, PlayerPos>(_consumerConfig)
        //    .SetValueDeserializer(new AvroDeserializer<PlayerPos>(_schemaRegistry).AsSyncOverAsync())
        //    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
        //    .Build();
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    public Task Consume(string topic, IConsumer.ProcessMessage action, CancellationToken ct)
    {
        _consumer.Subscribe(topic);
        while (!ct.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(ct);
            var result = consumeResult.Message;
            Console.WriteLine(
                $"{result.Key} = {result.Value} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            //action(result.Key, result.Value);
        }

        _consumer.Close();
        return Task.CompletedTask;
    }
}