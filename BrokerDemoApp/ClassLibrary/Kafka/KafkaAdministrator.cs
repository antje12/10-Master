using ClassLibrary.Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace ClassLibrary.Kafka;

public class KafkaAdministrator : IAdministrator
{
    private readonly IAdminClient _adminClient;
    
    public KafkaAdministrator(AdminClientConfig adminClientConfig)
    {
        _adminClient = new AdminClientBuilder(adminClientConfig).Build();
    }

    public async Task CreateTopic(string topic)
    {
        try
        {
            var metadata = _adminClient.GetMetadata(TimeSpan.FromSeconds(1));
            if (metadata.Topics.All(x => x.Topic != topic))
            {
                await _adminClient.CreateTopicsAsync(new TopicSpecification[]
                {
                    new TopicSpecification {Name = topic, ReplicationFactor = 1, NumPartitions = 1}
                });
            }
        }
        catch (CreateTopicsException e)
        {
            Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
        }
    }
}