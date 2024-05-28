using Avro.Specific;
using ClassLibrary.Kafka;

namespace ClassLibrary.Interfaces;

public interface IAvroConsumer<T> where T : ISpecificRecord
{
    delegate void ProcessMessage(string key, T value);
    Task Consume(KafkaTopic topic, ProcessMessage action, CancellationToken ct);
    Task Consume(string topic, ProcessMessage action, CancellationToken ct);
}