// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;

Console.WriteLine("Hello, World!");

ProducerConfig _producerConfig;

//const string KafkaServers = "kafka-1:9092,kafka-2:9092,kafka-3:9092"; // internal docker call
const string KafkaServers = "localhost:19092"; //,localhost:29092,localhost:39092"; // external docker call
//const string SchemaRegistry = "http://schema-registry:8081"; // internal docker call

_producerConfig = new ProducerConfig
{
    BootstrapServers = KafkaServers,
    Acks = Acks.None,
    LingerMs = 0,
    BatchSize = 1
};

using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
while (true)
{
    string message = Console.ReadLine();
    Console.WriteLine($"{message} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
    producer.Produce("msg-topic", new Message<Null, string>
    {
        Value = message
    });
    producer.Flush();
}