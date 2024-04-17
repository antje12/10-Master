using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using NUnit.Framework;
using Moq;

namespace InputService.Tests;

[TestFixture]
public class Test
{
    private Mock<IAdministrator> admin;
    private Mock<IProtoConsumer<Input_M>> consumer;
    private Mock<IProtoProducer<Collision_M>> producerC;
    private Mock<IProtoProducer<World_M>> producerW;
    private Services.InputService service;

    [SetUp]
    public void Setup()
    {
        admin = new Mock<IAdministrator>();
        admin.Setup(x => x.CreateTopic(It.IsAny<KafkaTopic>()))
            .Returns(Task.CompletedTask);

        consumer = new Mock<IProtoConsumer<Input_M>>();
        consumer.Setup(x => x.Consume(
                It.IsAny<KafkaTopic>(),
                It.IsAny<IProtoConsumer<Input_M>.ProcessMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () => { await Task.Delay(500); });
        
        producerC = new Mock<IProtoProducer<Collision_M>>();
        producerC.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(), 
            It.IsAny<string>(), 
            It.IsAny<Collision_M>()));
        
        producerW = new Mock<IProtoProducer<World_M>>();
        producerW.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(), 
            It.IsAny<string>(), 
            It.IsAny<World_M>()));
        
        service = new Services.InputService();
        service.Admin = admin.Object;
        service.Consumer = consumer.Object;
        service.ProducerC = producerC.Object;
        service.ProducerW = producerW.Object;
    }

    [Test]
    public async Task TestExecuteAsync()
    {
        var executeTask =  service.ExecuteAsync();
        await Task.Delay(100);
        Assert.That(service.IsRunning, Is.False);
        await executeTask;

        admin.Verify(x => x.CreateTopic(KafkaTopic.Input));
        consumer.Verify(x => x.Consume(
            KafkaTopic.Input,
            It.IsAny<IProtoConsumer<Input_M>.ProcessMessage>(),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    [TestCase(GameKey.Up)]
    [TestCase(GameKey.Down)]
    [TestCase(GameKey.Left)]
    [TestCase(GameKey.Right)]
    [TestCase(GameKey.Up, GameKey.Right)]
    [TestCase(GameKey.Up, GameKey.Left)]
    [TestCase(GameKey.Down, GameKey.Right)]
    [TestCase(GameKey.Down, GameKey.Left)]
    public void TestMoves(params GameKey[] keys)
    {
        var key = "test";
        var value = new Input_M
        {
            AgentId = key,
            AgentLocation = new Coordinates_M {X = 0, Y = 0},
            MouseLocation = new Coordinates_M {X = 0, Y = 0},
            GameTime = 0.0166667, // monogame = 60 updates a second
            EventId = key,
            Source = Source.Player
        };
        value.KeyInput.AddRange(keys);
        service.ProcessMessage(key, value);
        
        ClassLibrary.GameLogic.Move.Agent(value.AgentLocation.X, value.AgentLocation.Y, value.KeyInput.ToList(),
            value.GameTime,
            out float toX, out float toY);
        var msgOut = new Collision_M()
        {
            EntityId = value.AgentId,
            EntityType = value.Source == Source.Ai ? EntityType.Ai : EntityType.Player,
            
            FromLocation = value.AgentLocation,
            ToLocation = new()
            {
                X = toX,
                Y = toY
            },
            LastUpdate = value.LastUpdate,
            EventId = value.EventId
        };
        
        producerC.Verify(x => 
            x.Produce(KafkaTopic.Collision, key, msgOut), 
            Times.Exactly(1));
    }
    
    [Test]
    [TestCase(GameKey.Up)]
    public void TestMoveLogic(params GameKey[] keys)
    {
        ClassLibrary.GameLogic.Move.Agent(
            0, 0, new List<GameKey>() {GameKey.Up},
            0.0166667,
            out float toX, out float toY);

        Assert.That(toX, Is.EqualTo(0));
        Assert.That(toY, Is.EqualTo(-100*0.0166667f));
    }

    [Test]
    [TestCase(GameKey.Attack)]
    public void TestAttack(params GameKey[] keys)
    {
        var key = "test";
        var value = new Input_M
        {
            AgentId = key,
            AgentLocation = new Coordinates_M {X = 0, Y = 0},
            MouseLocation = new Coordinates_M {X = 10, Y = 10},
            GameTime = 0.0166667,
            EventId = key,
            Source = Source.Player
        };
        value.KeyInput.AddRange(keys);
        
        World_M msgOut = null;
        producerW.Setup(x => 
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()))
            .Callback<KafkaTopic, string, World_M>((topic, entityId, msg) => msgOut = msg);
        
        service.ProcessMessage(key, value);
        
        Assert.That(msgOut, Is.Not.Null);
        Assert.That(msgOut.Change, Is.EqualTo(Change.SpawnBullet));
        Assert.That(msgOut.Location.X, Is.EqualTo(28.2842712f));
        Assert.That(msgOut.Location.Y, Is.EqualTo(28.2842712f));
        Assert.That(msgOut.Direction.X, Is.EqualTo(0.707106769f));
        Assert.That(msgOut.Direction.Y, Is.EqualTo(0.707106769f));
        
        producerW.Verify(x => 
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()), 
            Times.Exactly(1));
    }

    [Test]
    [TestCase(GameKey.Attack)]
    public async Task TestAttacks(params GameKey[] keys)
    {
        var key = "test";
        var value = new Input_M
        {
            AgentId = key,
            AgentLocation = new Coordinates_M {X = 0, Y = 0},
            MouseLocation = new Coordinates_M {X = 10, Y = 10},
            GameTime = 0.0166667,
            EventId = key,
            Source = Source.Player
        };
        value.KeyInput.AddRange(keys);
        
        service.ProcessMessage(key, value);
        service.ProcessMessage(key, value);
        
        producerW.Verify(x => 
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()), 
            Times.Exactly(1));
        
        await Task.Delay(1100);
        service.ProcessMessage(key, value);
        
        producerW.Verify(x => 
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()), 
            Times.Exactly(2));
    }

    [Test]
    [TestCase(GameKey.Interact)]
    public void TestInteract(params GameKey[] keys)
    {
        var key = "test";
        var value = new Input_M
        {
            AgentId = key,
            AgentLocation = new Coordinates_M {X = 0, Y = 0},
            MouseLocation = new Coordinates_M {X = 0, Y = 0},
            GameTime = 0.0166667,
            EventId = key,
            Source = Source.Player
        };
        value.KeyInput.Add(GameKey.Interact);
        service.ProcessMessage(key, value);
        
        producerW.Verify(x => 
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()), 
            Times.Exactly(1));
    }
}