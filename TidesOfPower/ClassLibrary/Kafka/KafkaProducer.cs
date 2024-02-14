using Avro.Specific;
using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary.Kafka;

public class KafkaProducer<T> : IProducer<T> where T : ISpecificRecord
{
    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IProducer<string, T> _producer;

    public KafkaProducer(
        ProducerConfig producerConfig,
        SchemaRegistryConfig schemaRegistryConfig,
        AvroSerializerConfig avroSerializerConfig)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        _producer = new ProducerBuilder<string, T>(producerConfig)
            .SetValueSerializer(new AvroSerializer<T>(_schemaRegistry, avroSerializerConfig))
            .Build();
        //_producer = new ProducerBuilder<string, T>(producerConfig).Build();
    }

    public void Produce(string topic, string key, T value)
    {
        Console.WriteLine($"{key} = {value.Get(0)} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        var result = _producer.ProduceAsync(topic, new Message<string, T>
        {
            Key = key,
            Value = value
        }).Result.Value;
        _producer.Flush();
    }
}