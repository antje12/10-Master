namespace ClassLibrary.Interfaces;

public interface IConsumer
{
    delegate void ProcessMessage (string key, string value);
    Task StartConsumer(string topic, ProcessMessage action);
}