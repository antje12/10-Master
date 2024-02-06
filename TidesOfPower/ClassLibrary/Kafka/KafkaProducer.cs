using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace ClassLibrary.Kafka;

public class KafkaProducer : IProducer
{
    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(
        ProducerConfig producerConfig,
        SchemaRegistryConfig schemaRegistryConfig)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        //_producer = new ProducerBuilder<string, PlayerPos>(_producerConfig)
        //    .SetValueSerializer(new AvroSerializer<PlayerPos>(_schemaRegistry, _avroSerializerConfig))
        //    .Build();
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public void Produce(string topic, string key, string value)
    {
        Console.WriteLine($"{key} = {value} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        _producer.Produce(topic, new Message<string, string>
        {
            Key = key,
            Value = value
        });
        _producer.Flush();
    }
}