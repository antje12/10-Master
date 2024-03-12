﻿using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using InputService.Interfaces;
using ClassLibrary.Messages.Protobuf;

namespace InputService.Services;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
//https://medium.com/simform-engineering/creating-microservices-with-net-core-and-kafka-a-step-by-step-approach-1737410ba76a
public class InputService : BackgroundService, IConsumerService
{
    private const string GroupId = "input-group";
    private KafkaTopic InputTopic = KafkaTopic.Input;
    private KafkaTopic OutputTopicC = KafkaTopic.Collision;
    private KafkaTopic OutputTopicW = KafkaTopic.World;

    private readonly KafkaAdministrator _admin;
    private readonly ProtoKafkaProducer<CollisionCheck> _producerC;
    private readonly ProtoKafkaProducer<WorldChange> _producerW;
    private readonly ProtoKafkaConsumer<Input> _consumer;

    public bool IsRunning { get; private set; }

    public InputService()
    {
        Console.WriteLine($"InputService created");
        var config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(config);
        _producerC = new ProtoKafkaProducer<CollisionCheck>(config);
        _producerW = new ProtoKafkaProducer<WorldChange>(config);
        _consumer = new ProtoKafkaConsumer<Input>(config);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"InputService started");

        await _admin.CreateTopic(InputTopic);
        IProtoConsumer<Input>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume(InputTopic, action, ct);

        IsRunning = false;
        Console.WriteLine($"InputService stopped");
    }

    private void ProcessMessage(string key, Input value)
    {
        //var stopwatch = new Stopwatch();
        //stopwatch.Start();
        
        if (value.KeyInput.Any(x => x is GameKey.Up or GameKey.Down or GameKey.Left or GameKey.Right))
            Move(key, value);

        if (value.KeyInput.Any(x => x is GameKey.Attack))
            Attack(key, value);

        if (value.KeyInput.Any(x => x is GameKey.Interact))
            Interact(key, value);
        
        //stopwatch.Stop();
        //var elapsedTime = stopwatch.ElapsedMilliseconds;
        //if (elapsedTime > 20) Console.WriteLine($"Message processed in {elapsedTime} ms");
    }

    private void Move(string key, Input value)
    {
        string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        Console.WriteLine($"Got {value.EventId} at {timestampWithMs}");
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var output = new CollisionCheck()
        {
            EntityId = value.PlayerId,
            Entity = EntityType.Avatar,
            FromLocation = value.PlayerLocation,
            ToLocation = new Coordinates()
            {
                X = value.PlayerLocation.X,
                Y = value.PlayerLocation.Y
            },
            EventId = value.EventId
        };

        foreach (var input in value.KeyInput)
        {
            switch (input)
            {
                case GameKey.Up:
                    output.ToLocation.Y -= 100 * (float) value.GameTime;
                    break;
                case GameKey.Down:
                    output.ToLocation.Y += 100 * (float) value.GameTime;
                    break;
                case GameKey.Left:
                    output.ToLocation.X -= 100 * (float) value.GameTime;
                    break;
                case GameKey.Right:
                    output.ToLocation.X += 100 * (float) value.GameTime;
                    break;
            }
        }

        _producerC.Produce(OutputTopicC, key, output);
        
        timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        Console.WriteLine($"Send {output.EventId} at {timestampWithMs}");
        
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        if (value.EventId != "") Console.WriteLine($"Message processed in {elapsedTime} ms -- {value.EventId}");
    }

    private void Attack(string key, Input value)
    {
        var x = value.MouseLocation.X - value.PlayerLocation.X;
        var y = value.MouseLocation.Y - value.PlayerLocation.Y;
        var length = (float) Math.Sqrt(x * x + y * y);
        if (length > 0)
        {
            x /= length;
            y /= length;
        }
        
        var spawnX = value.PlayerLocation.X + x * (25 + 5 + 1);
        var spawnY = value.PlayerLocation.Y + y * (25 + 5 + 1);
        
        var output = new WorldChange()
        {
            EntityId = Guid.NewGuid().ToString(),
            Change = ChangeType.SpawnBullet,
            Location = new Coordinates() { X = spawnX, Y = spawnY },
            Direction = new Coordinates() {X = x, Y = y}
        };

        _producerW.Produce(OutputTopicW, key, output);
    }

    private void Interact(string key, Input value)
    {
    }
}