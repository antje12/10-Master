namespace CollisionService.Interfaces;

public interface IConsumerService : IHostedService
{
    bool IsRunning { get; }
}