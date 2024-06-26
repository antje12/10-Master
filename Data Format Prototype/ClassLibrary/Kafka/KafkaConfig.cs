﻿using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ClassLibrary.Kafka;

public class KafkaConfig
{
    //client:           "localhost:19092"
    //kubernetes:       "kafka-service:9092"
    private string _kafkaServers = "kafka-service:9092";
    //client:           "localhost:8081"
    //kubernetes:       "schema-registry-service:8081"
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
            _kafkaServers = "192.168.1.106:19092";
            _schemaRegistry = "192.168.1.106:8081";
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