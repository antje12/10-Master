using ClassLibrary.Interfaces;
using Confluent.Kafka;

namespace ClassLibrary.Kafka;

public class KafkaProducer : IProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(ProducerConfig producerConfig)
    {
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public void Produce(string topic, string key, string value)
    {
        //Console.WriteLine($"{key} = {value} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        var result = _producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = key,
            Value = value
        }).Result.Value;
        _producer.Flush();
    }
}