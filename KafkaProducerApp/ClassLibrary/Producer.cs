using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary;

public class Producer
{
    private readonly ProducerConfig _producerConfig;
    private readonly SchemaRegistryConfig _schemaRegistryConfig;
    private readonly AvroSerializerConfig _avroSerializerConfig;

    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IProducer<string, PlayerPos> _producer;

    public Producer(
        ProducerConfig producerConfig,
        SchemaRegistryConfig schemaRegistryConfig,
        AvroSerializerConfig avroSerializerConfig)
    {
        _producerConfig = producerConfig;
        _schemaRegistryConfig = schemaRegistryConfig;
        _avroSerializerConfig = avroSerializerConfig;

        _schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
        _producer = new ProducerBuilder<string, PlayerPos>(_producerConfig)
            .SetValueSerializer(new AvroSerializer<PlayerPos>(_schemaRegistry, _avroSerializerConfig))
            .Build();
    }

    public string Produce(string topic, PlayerPos message)
    {
        var result = _producer.ProduceAsync(topic, new Message<string, PlayerPos>
        {
            Key = message.ID,
            Value = message
        }).Result.Value;
        _producer.Flush();

        return result.ID;
    }
}