using Avro.Specific;

namespace ClassLibrary.Interfaces;

public interface IConsumer<T> where T : ISpecificRecord
{
    delegate void ProcessMessage(string key, T value);

    Task Consume(string topic, ProcessMessage action, CancellationToken ct);
}