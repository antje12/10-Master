using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary.Kafka;

public class KafkaConfig
{
    //client:           "localhost:19092"
    //kubernetes:       "kafka-service:9092"
    //google:           "34.32.47.73:30001"
    private string _kafkaServers = "kafka-service-1:9092,kafka-service-2:9092,kafka-service-3:9092";
    //client:           "localhost:8081"
    //kubernetes:       "schema-registry-service:8081"
    //google:           "34.32.47.73:30004"
    private string _schemaRegistry = "schema-registry-service:8081";

    public SchemaRegistryConfig SchemaRegistryConfig;
    public AvroSerializerConfig AvroSerializerConfig;
    public AdminClientConfig AdminConfig;
    public ProducerConfig ProducerConfig;
    public ConsumerConfig ConsumerConfig;

    public KafkaConfig(string groupId, bool isClient = false)
    {
        if (isClient)
        {
            _kafkaServers = "34.32.47.73:30001,34.32.47.73:30002,34.32.47.73:30003";
            _schemaRegistry = "34.32.47.73:30004";
        }
        SchemaRegistryConfig = new()
        {
            Url = _schemaRegistry
        };
        AvroSerializerConfig = new()
        {
            BufferBytes = 100
        };
        AdminConfig = new()
        {
            BootstrapServers = _kafkaServers
        };
        ProducerConfig = new()
        {
            BootstrapServers = _kafkaServers,
            AllowAutoCreateTopics = false,
            Acks = Acks.None,
            LingerMs = 0,
            BatchSize = 1
        };
        ConsumerConfig = new()
        {
            BootstrapServers = _kafkaServers,
            AllowAutoCreateTopics = false,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SessionTimeoutMs = 6000,
            ConsumeResultFields = "none"
        };
    }
}