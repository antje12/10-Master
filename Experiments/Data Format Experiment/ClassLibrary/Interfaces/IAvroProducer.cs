using Avro.Specific;
using ClassLibrary.Kafka;

namespace ClassLibrary.Interfaces;

public interface IAvroProducer<T> where T : ISpecificRecord
{
    void Produce(KafkaTopic topic, string key, T value);
    void Produce(string topic, string key, T value);
}