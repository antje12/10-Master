namespace TickService.Interfaces;

public interface IConsumerService : IHostedService
{
    bool IsRunning { get; }
}