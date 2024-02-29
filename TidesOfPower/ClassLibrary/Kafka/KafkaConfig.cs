using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary.Kafka;

public class KafkaConfig
{
    //client:           "localhost:19092"
    //docker compose:   "kafka-1:9092"
    //kubernetes:       "kafka-service:9092"
    private string KafkaServers = "kafka-1:9092";
    //client:           "localhost:8081"
    //docker compose:   "schema-registry:8081"
    //kubernetes:       "schema-registry-service:8081"
    private string SchemaRegistry = "schema-registry:8081";

    public readonly SchemaRegistryConfig SchemaRegistryConfig;
    public readonly AvroSerializerConfig AvroSerializerConfig;
    public readonly AdminClientConfig AdminConfig;
    public readonly ProducerConfig ProducerConfig;
    public readonly ConsumerConfig ConsumerConfig;

    public KafkaConfig(string groupId, bool client = false)
    {
        if (client)
        {
            KafkaServers = "localhost:19092";
            SchemaRegistry = "localhost:8081";
        }
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