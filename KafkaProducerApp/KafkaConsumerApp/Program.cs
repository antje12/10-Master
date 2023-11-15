// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Confluent.Kafka.SyncOverAsync;
using KafkaProducerApp.DTOs;

Console.WriteLine("Hello, World!");

ProducerConfig _producerConfig;
ConsumerConfig _consumerConfig;
SchemaRegistryConfig _schemaRegistryConfig;

//const string KafkaServers = "kafka-1:9092,kafka-2:9092,kafka-3:9092"; // internal docker call
const string KafkaServers = "localhost:19092,localhost:29092,localhost:39092"; // external docker call
const string GroupId = "KafkaPlugin";
//const string SchemaRegistry = "http://schema-registry:8081"; // internal docker call
const string SchemaRegistry = "localhost:8081"; // external docker call

_consumerConfig = new ConsumerConfig
{
    BootstrapServers = KafkaServers,
    GroupId = GroupId,
    AutoOffsetReset = AutoOffsetReset.Earliest
};
_schemaRegistryConfig = new SchemaRegistryConfig
{
    Url = SchemaRegistry
};

using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
using (var consumer = new ConsumerBuilder<string, Message>(_consumerConfig)
           .SetValueDeserializer(new AvroDeserializer<Message>(schemaRegistry).AsSyncOverAsync())
           .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
           .Build())
{
    consumer.Subscribe("msg-topic");
    while (true)
    {
        var consumeResult = consumer.Consume();
        var result = consumeResult.Message.Value;
        Console.WriteLine($"{result.ID} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
        Thread.Sleep(1000);
        //consumer.Close();
    }
}