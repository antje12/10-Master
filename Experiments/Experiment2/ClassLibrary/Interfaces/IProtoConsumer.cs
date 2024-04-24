using Avro.Specific;
using ClassLibrary.Kafka;
using Google.Protobuf;

namespace ClassLibrary.Interfaces;

public interface IProtoConsumer<T> where T : class, IMessage<T>, new()
{
    delegate void ProcessMessage(string key, T value);
    Task Consume(KafkaTopic topic, ProcessMessage action, CancellationToken ct);
    Task Consume(string topic, ProcessMessage action, CancellationToken ct);
}