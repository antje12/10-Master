using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary.Kafka;

public class JsonKafkaProducer<T> : IJsonProducer<T> where T : class
{
    private CachedSchemaRegistryClient _schemaRegistry;
    private IProducer<string, T> _producer;

    public JsonKafkaProducer(KafkaConfig config)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(config.SchemaRegistryConfig);
        _producer = new ProducerBuilder<string, T>(config.ProducerConfig)
            .SetValueSerializer(new JsonSerializer<T>(_schemaRegistry))
            .SetErrorHandler((_, e) => Console.WriteLine($"Error producing to topic: {e.Reason}"))
            .Build();
        //_producer = new ProducerBuilder<string, T>(producerConfig).Build();
    }

    public void Produce(KafkaTopic topic, string key, T value)
    {
        Produce(topic.ToString(), key, value);
    }

    public void Produce(string topic, string key, T value)
    {
        //Console.WriteLine(
        //    $"{topic}: {key} = {value} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        var result = _producer.ProduceAsync(topic, new Message<string, T>
        {
            Key = key,
            Value = value
        }).Result.Value;
        _producer.Flush();
    }
}