namespace AIService.Interfaces;

public interface IConsumerService : IHostedService
{
    bool IsRunning { get; }
}