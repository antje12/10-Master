// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;

Console.WriteLine("Hello, World!");

ConsumerConfig _consumerConfig;

//const string KafkaServers = "kafka-1:9092,kafka-2:9092,kafka-3:9092"; // internal docker call
const string KafkaServers = "localhost:19092"; //,localhost:29092,localhost:39092"; // external docker call
const string GroupId = "KafkaPlugin";
//const string SchemaRegistry = "http://schema-registry:8081"; // internal docker call

_consumerConfig = new ConsumerConfig
{
    BootstrapServers = KafkaServers,
    GroupId = GroupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    FetchMinBytes = 1,
    FetchWaitMaxMs = 100
};

using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
consumer.Subscribe("msg-topic");
while (true)
{
    var consumeResult = consumer.Consume();
    var result = consumeResult.Message.Value;
    Console.WriteLine($"{result} consumed - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
    //consumer.Close();
}