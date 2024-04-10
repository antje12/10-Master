using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Google.Protobuf;

namespace ClassLibrary.Kafka;

public class KafkaProducer<T> : IProtoProducer<T> where T : class, IMessage<T>, new()
{
    private CachedSchemaRegistryClient _schemaRegistry;
    private IProducer<string, T> _producer;

    public KafkaProducer(KafkaConfig config)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(config.SchemaRegistryConfig);
        _producer = new ProducerBuilder<string, T>(config.ProducerConfig)
            .SetValueSerializer(new ProtobufSerializer<T>(_schemaRegistry))
            .SetErrorHandler((_, e) => Console.WriteLine($"Error producing to topic: {e.Reason}"))
            .Build();
    }

    public void Produce(KafkaTopic topic, string key, T value)
    {
        Produce(topic.ToString(), key, value);
    }

    public void Produce(string topic, string key, T value)
    {
        var result = _producer.ProduceAsync(topic, new Message<string, T>
        {
            Key = key,
            Value = value
        }).Result.Value;
        _producer.Flush();
    }
}