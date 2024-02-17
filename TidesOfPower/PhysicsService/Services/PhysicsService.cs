﻿using ClassLibrary.Classes.Client;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using PhysicsService.Interfaces;

namespace PhysicsService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class PhysicsService : BackgroundService, IConsumerService
{
    private const string GroupId = "physics-group";

    private readonly KafkaAdministrator _admin;
    private readonly KafkaProducer<LocalState> _producer;
    private readonly KafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public PhysicsService()
    {
        Console.WriteLine($"PhysicsService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _admin.CreateTopic(KafkaTopic.Physics);
        _producer = new KafkaProducer<LocalState>(config);
        _consumer = new KafkaConsumer<Input>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"PhysicsService started");

        await _admin.CreateTopic(KafkaTopic.Physics);
        IConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(KafkaTopic.Physics, action, ct);

        IsRunning = false;
        Console.WriteLine($"PhysicsService stopped");
    }

    private void ProcessMessage(string key, Input value)
    {
        var output = new LocalState()
        {
            PlayerId = value.PlayerId,
            Location = value.Location
        };

        //_producer.Produce($"{KafkaTopic.LocalState}_{output.PlayerId.ToString()}", key, output);
        _producer.Produce(KafkaTopic.LocalState.ToString(), key, output);
    }
}