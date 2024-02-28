using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary.Kafka;

public class KafkaConfig
{
    //"localhost:19092"
    //"kafka-1:9092"
    private const string KafkaServers = "kafka-1:9092";
    //"localhost:8081"
    //"schema-registry:8081"
    private const string SchemaRegistry = "schema-registry:8081";

    public readonly SchemaRegistryConfig SchemaRegistryConfig;
    public readonly AvroSerializerConfig AvroSerializerConfig;
    public readonly AdminClientConfig AdminConfig;
    public readonly ProducerConfig ProducerConfig;
    public readonly ConsumerConfig ConsumerConfig;

    public KafkaConfig(string groupId)
    {
        SchemaRegistryConfig = new()
        {
            Url = SchemaRegistry
        };
        AvroSerializerConfig = new()
        {
            BufferBytes = 100
        };
        AdminConfig = new()
        {
            BootstrapServers = KafkaServers
        };
        ProducerConfig = new()
        {
            BootstrapServers = KafkaServers,
            Acks = Acks.None,
            LingerMs = 0,
            BatchSize = 1
        };
        ConsumerConfig = new()
        {
            BootstrapServers = KafkaServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SessionTimeoutMs = 6000,
            ConsumeResultFields = "none"
        };
    }
}