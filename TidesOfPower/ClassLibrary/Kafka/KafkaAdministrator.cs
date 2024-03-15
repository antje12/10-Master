using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace ClassLibrary.Kafka;

public class KafkaAdministrator : IAdministrator
{
    private IAdminClient _adminClient;

    public KafkaAdministrator(KafkaConfig config)
    {
        _adminClient = new AdminClientBuilder(config.AdminConfig)
            .SetErrorHandler((_, e) => Console.WriteLine($"Error administering topic: {e.Reason}"))
            .Build();
    }

    public async Task CreateTopic(KafkaTopic topic)
    {
        await CreateTopic(topic.ToString());
    }

    public async Task CreateTopic(string topic)
    {
        try
        {
            var metadata = _adminClient.GetMetadata(TimeSpan.FromSeconds(1));
            if (metadata.Topics.All(x => x.Topic != topic.ToString()))
            {
                await _adminClient.CreateTopicsAsync(new TopicSpecification[]
                {
                    new() {Name = topic, ReplicationFactor = 1, NumPartitions = 10}
                });
            }
        }
        catch (CreateTopicsException e)
        {
            Console.WriteLine($"Error creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
        }
    }
}