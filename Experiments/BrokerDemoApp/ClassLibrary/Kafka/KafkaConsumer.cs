using ClassLibrary.Interfaces;
using Confluent.Kafka;

namespace ClassLibrary.Kafka;

public class KafkaConsumer : IConsumer
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumer(
        ConsumerConfig consumerConfig,
        CancellationTokenSource cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    public Task StartConsumer(string topic, IConsumer.ProcessMessage action)
    {
        return Task.Run(() => ConsumeLoop(topic, action), _cancellationTokenSource.Token);
    }

    private Task ConsumeLoop(string topic, IConsumer.ProcessMessage onMessage)
    {
        _consumer.Subscribe(topic);
        while (true)
        {
            var consumeResult = _consumer.Consume();
            var result = consumeResult.Message;
            Console.WriteLine(
                $"{result.Key} = {result.Value} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            onMessage(result.Key, result.Value);
        }

        _consumer.Close();
        return Task.CompletedTask;
    }

    public void StopConsumer()
    {
        _cancellationTokenSource.Cancel();
    }
}