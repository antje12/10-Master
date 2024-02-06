namespace ClassLibrary.Interfaces;

public interface IConsumer
{
    delegate void ProcessMessage (string key, string value);
    Task Consume(string topic, ProcessMessage action, CancellationTokenSource cts);
}