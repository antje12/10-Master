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
    private readonly IProducer<string, string> _producer;

    public Producer(
        ProducerConfig producerConfig,
        SchemaRegistryConfig schemaRegistryConfig,
        AvroSerializerConfig avroSerializerConfig)
    {
        _producerConfig = producerConfig;
        _schemaRegistryConfig = schemaRegistryConfig;
        _avroSerializerConfig = avroSerializerConfig;

        _schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
        //_producer = new ProducerBuilder<string, PlayerPos>(_producerConfig)
        //    .SetValueSerializer(new AvroSerializer<PlayerPos>(_schemaRegistry, _avroSerializerConfig))
        //    .Build();
        _producer = new ProducerBuilder<string, string>(_producerConfig).Build();
    }

    public void Produce(string topic, string key, string message)
    {
        Console.WriteLine($"{key} = {message} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        _producer.Produce(topic, new Message<string, string>
        {
            Key = key,
            Value = message
        });
        _producer.Flush();
    }
}