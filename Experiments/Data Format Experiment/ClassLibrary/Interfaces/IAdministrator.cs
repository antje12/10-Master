using ClassLibrary.Kafka;

namespace ClassLibrary.Interfaces;

public interface IAdministrator
{
    Task CreateTopic(KafkaTopic topic);
    Task CreateTopic(string topic);
}