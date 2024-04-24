using Avro.Specific;
using ClassLibrary.Kafka;
using Google.Protobuf;

namespace ClassLibrary.Interfaces;

public interface IProtoProducer<T> where T : class, IMessage<T>, new()
{
    void Produce(KafkaTopic topic, string key, T value);
    void Produce(string topic, string key, T value);
}