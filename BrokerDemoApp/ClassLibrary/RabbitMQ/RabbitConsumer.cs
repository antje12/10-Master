using System.Text;
using ClassLibrary.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassLibrary.RabbitMQ;

public class RabbitConsumer : IConsumer
{
    private readonly IChannel _channel;
    
    public RabbitConsumer(string topic)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateChannel();
        _channel.QueueDeclare(queue: topic,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public Task StartConsumer(string topic, IConsumer.ProcessMessage onMessage)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);Console.WriteLine(
                $"{topic} = {message} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            onMessage(topic, message);
        };
        _channel.BasicConsume(queue: topic,
            autoAck: true,
            consumer: consumer);

        while (true)
        {
        }
    }
}