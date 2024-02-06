using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace ClassLibrary.Kafka;

public class KafkaConsumer : IConsumer
{
    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumer(
        ConsumerConfig consumerConfig,
        SchemaRegistryConfig schemaRegistryConfig)
    {
        _schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        //_consumer = new ConsumerBuilder<string, PlayerPos>(_consumerConfig)
        //    .SetValueDeserializer(new AvroDeserializer<PlayerPos>(_schemaRegistry).AsSyncOverAsync())
        //    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
        //    .Build();
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    public Task Consume(string topic, IConsumer.ProcessMessage action, CancellationTokenSource cts)
    {
        return Task.Run(() => ConsumeLoop(topic, action, cts.Token), cts.Token);
    }

    private Task ConsumeLoop(string topic, IConsumer.ProcessMessage onMessage, CancellationToken ct)
    {
        Console.WriteLine($"Starting consumption loop");
        _consumer.Subscribe(topic);
        while (!ct.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(ct);
            var result = consumeResult.Message;
            Console.WriteLine(
                $"{result.Key} = {result.Value} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
            //onMessage(result.Key, result.Value);
        }

        _consumer.Close();
        Console.WriteLine($"Ended consumption loop");
        return Task.CompletedTask;
    }
}