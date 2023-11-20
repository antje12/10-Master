using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassLibrary;

public class RabbitConsumer
{
    public delegate void OnMessage (string key, string message);

    public void StartConsumer(string topic, OnMessage onMessage)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateChannel();

        channel.QueueDeclare(queue: topic,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);Console.WriteLine(
                $"{topic} = {message} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            onMessage(topic, message);
        };
        channel.BasicConsume(queue: topic,
            autoAck: true,
            consumer: consumer);

        while (true)
        {
        }
    }
}