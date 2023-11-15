// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaProducerApp.DTOs;

Console.WriteLine("Hello, World!");

ProducerConfig _producerConfig;
SchemaRegistryConfig _schemaRegistryConfig;
AvroSerializerConfig _avroSerializerConfig;

//const string KafkaServers = "kafka-1:9092,kafka-2:9092,kafka-3:9092"; // internal docker call
const string KafkaServers = "localhost:19092,localhost:29092,localhost:39092"; // external docker call
//const string SchemaRegistry = "http://schema-registry:8081"; // internal docker call
const string SchemaRegistry = "localhost:8081"; // external docker call

_producerConfig = new ProducerConfig
{
    BootstrapServers = KafkaServers
};
_schemaRegistryConfig = new SchemaRegistryConfig
{
    Url = SchemaRegistry
};
_avroSerializerConfig = new AvroSerializerConfig
{
    BufferBytes = 100
};

Message result;
using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
using (var producer = new ProducerBuilder<string, Message>(_producerConfig)
           .SetValueSerializer(new AvroSerializer<Message>(schemaRegistry, _avroSerializerConfig))
           .Build())
{
    var message = new Message()
    {
        ID = Guid.NewGuid().ToString(),
        Num1 = "1",
        Num2 = "2"
    };
    
    Console.WriteLine($"{message.ID} produced - {DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.fff")}");
    result = producer.ProduceAsync("msg-topic", new Message<string, Message>
        {
            Key = message.ID,
            Value = message
        }).Result
        .Value;
}