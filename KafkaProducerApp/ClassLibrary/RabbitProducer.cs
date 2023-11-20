using System.Text;
using RabbitMQ.Client;

namespace ClassLibrary;

public class RabbitProducer
{
    public void Produce(string topic, string message)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateChannel();

        channel.QueueDeclare(queue: topic,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        Console.WriteLine($"{topic} = {message} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: topic,
            body: body);
    }
}