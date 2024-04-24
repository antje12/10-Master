using Avro.Specific;
using ClassLibrary.Kafka;
using Google.Protobuf;

namespace ClassLibrary.Interfaces;

public interface IJsonConsumer<T> where T : class
{
    delegate void ProcessMessage(string key, T value);
    Task Consume(KafkaTopic topic, ProcessMessage action, CancellationToken ct);
    Task Consume(string topic, ProcessMessage action, CancellationToken ct);
}