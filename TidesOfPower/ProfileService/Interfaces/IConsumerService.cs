namespace ProfileService.Interfaces;

public interface IConsumerService : IHostedService
{
    bool IsRunning { get; }
}