using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace ClassLibrary;

public class KafkaAdministrator
{
    private readonly AdminClientConfig _adminClientConfig;
    private readonly IAdminClient _adminClient;
    
    public KafkaAdministrator(AdminClientConfig adminClientConfig)
    {
        _adminClientConfig = adminClientConfig;
        _adminClient = new AdminClientBuilder(_adminClientConfig).Build();
    }

    public async Task Setup(string topic)
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