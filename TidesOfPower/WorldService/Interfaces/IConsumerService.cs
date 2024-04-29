﻿namespace WorldService.Interfaces;

public interface IConsumerService : IHostedService
{
    bool IsRunning { get; }
    void StopService();
}