namespace PhysicsService.Interfaces;

public interface IConsumerService : IHostedService
{
    bool IsRunning { get; }
}