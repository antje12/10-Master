using Avro.Specific;

namespace ClassLibrary.Interfaces;

public interface IProducer<T> where T : ISpecificRecord
{
    void Produce(string topic, string key, T value);
}