namespace ClassLibrary.Interfaces;

public interface IProducer
{
    void Produce(string topic, string key, string value);
}