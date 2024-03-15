namespace ProjectileService.Interfaces;

public interface IConsumerService : IHostedService
{
    bool IsRunning { get; }
}