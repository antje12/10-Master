using System.Text;
using ClassLibrary.Interfaces;
using RabbitMQ.Client;

namespace ClassLibrary.RabbitMQ;

public class RabbitProducer : IProducer
{
    private readonly IChannel _channel;

    public RabbitProducer(string host, string topic)
    {
        var factory = new ConnectionFactory {HostName = host};
        var connection = factory.CreateConnection();
        _channel = connection.CreateChannel();
        _channel.QueueDeclare(queue: topic,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void Produce(string topic, string key, string value)
    {
        var body = Encoding.UTF8.GetBytes(value);
        //Console.WriteLine($"{topic} = {value} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: topic,
            body: body);
    }
}