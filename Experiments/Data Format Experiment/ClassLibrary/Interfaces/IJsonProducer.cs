using Avro.Specific;
using ClassLibrary.Kafka;
using Google.Protobuf;

namespace ClassLibrary.Interfaces;

public interface IJsonProducer<T> where T : class
{
    void Produce(KafkaTopic topic, string key, T value);
    void Produce(string topic, string key, T value);
}