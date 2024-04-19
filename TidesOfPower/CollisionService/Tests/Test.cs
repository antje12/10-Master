using ClassLibrary.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;
using NUnit.Framework;
using Moq;
using EntityType = ClassLibrary.Messages.Protobuf.EntityType;

namespace CollisionService.Tests;

[TestFixture]
public class Test
{
    private Mock<IAdministrator> admin;
    private Mock<IProtoConsumer<Collision_M>> consumer;
    private Mock<IProtoProducer<World_M>> producerW;
    private Mock<IProtoProducer<Ai_M>> producerA;
    private Mock<RedisBroker> redis;
    private Services.CollisionService service;

    [SetUp]
    public void Setup()
    {
        admin = new Mock<IAdministrator>();
        admin.Setup(x => x.CreateTopic(It.IsAny<KafkaTopic>()))
            .Returns(Task.CompletedTask);

        consumer = new Mock<IProtoConsumer<Collision_M>>();
        consumer.Setup(x => x.Consume(
                It.IsAny<KafkaTopic>(),
                It.IsAny<IProtoConsumer<Collision_M>.ProcessMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () => { await Task.Delay(500); });

        producerW = new Mock<IProtoProducer<World_M>>();
        producerW.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(),
            It.IsAny<string>(),
            It.IsAny<World_M>()));

        producerA = new Mock<IProtoProducer<Ai_M>>();
        producerA.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(),
            It.IsAny<string>(),
            It.IsAny<Ai_M>()));

        redis = new Mock<RedisBroker>();
        redis.Setup(x => x.Connect(false));
        redis.Setup(x => x.GetCloseEntities(
                It.IsAny<float>(),
                It.IsAny<float>()))
            .Returns(new List<Entity>()
            {
                new Agent(Guid.NewGuid(), new Coordinates(100, 100), ClassLibrary.Domain.EntityType.Player, 100, 100),
                new Projectile(new Coordinates(0, 0), 100, 100, 100, Guid.NewGuid(), new Coordinates(200, 200)),
                new Treasure(100, Guid.NewGuid(), new Coordinates(300, 300))
            });

        service = new Services.CollisionService();
        service.Admin = admin.Object;
        service.Consumer = consumer.Object;
        service.ProducerW = producerW.Object;
        service.ProducerA = producerA.Object;
        service.RedisBroker = redis.Object;
    }

    [Test]
    public async Task TestExecuteAsync()
    {
        var executeTask = service.ExecuteAsync();
        await Task.Delay(100);
        Assert.That(service.IsRunning, Is.True);
        await executeTask;

        admin.Verify(x => x.CreateTopic(KafkaTopic.Collision));
        consumer.Verify(x => x.Consume(
            KafkaTopic.Collision,
            It.IsAny<IProtoConsumer<Collision_M>.ProcessMessage>(),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public void TestPlayerCollisions()
    {
        var key = "test";
        var value = new Collision_M()
        {
            EntityId = key,
            EntityType = EntityType.Player,
            FromLocation = new() {X = 0, Y = 0},
            ToLocation = new() {X = 10, Y = 10},
            LastUpdate = DateTimeOffset.UtcNow.AddSeconds(-1).Ticks,
            EventId = key
        };
        
        World_M? temp = null;
        producerW.Setup(x => 
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()))
            .Callback<KafkaTopic, string, World_M>((topic, entityId, msg) => temp = msg);
        service.ProcessMessage(key, value);

        var msgOut = new World_M()
        {
            EntityId = value.EntityId,
            Location = value.ToLocation,
            Change = Change.MovePlayer,
            EventId = value.EventId,
            Value = 0
        };

        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, msgOut),
            Times.Exactly(1));

        value.ToLocation = new Coordinates_M() {X = 100, Y = 100};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(1));

        value.ToLocation = new Coordinates_M() {X = 200, Y = 200};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(2));

        value.ToLocation = new Coordinates_M() {X = 300, Y = 300};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(3));
        
        Assert.That(temp, Is.Not.Null);
        Assert.That(temp.Value, Is.EqualTo(100));
        
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, It.IsAny<string>(), It.IsAny<World_M>()),
            Times.Exactly(4)); // collect treasure (extra event)
    }

    [Test]
    public void TestAiCollisions()
    {
        var key = "test";
        var value = new Collision_M()
        {
            EntityId = key,
            EntityType = EntityType.Ai,
            FromLocation = new() {X = 0, Y = 0},
            ToLocation = new() {X = 10, Y = 10},
            LastUpdate = DateTimeOffset.UtcNow.AddSeconds(-1).Ticks,
            EventId = key
        };
        
        World_M? temp1 = null;
        producerW.Setup(x => 
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()))
            .Callback<KafkaTopic, string, World_M>((topic, entityId, msg) => temp1 = msg);
        
        Ai_M? temp2 = null;
        producerA.Setup(x => 
                x.Produce(KafkaTopic.Ai, key, It.IsAny<Ai_M>()))
            .Callback<KafkaTopic, string, Ai_M>((topic, entityId, msg) => temp2 = msg);
        
        service.ProcessMessage(key, value);

        var msgOut = new World_M()
        {
            EntityId = value.EntityId,
            Location = value.ToLocation,
            Change = Change.MoveAi,
            LastUpdate = value.LastUpdate
        };

        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, msgOut),
            Times.Exactly(1));

        value.ToLocation = new Coordinates_M() {X = 100, Y = 100};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(1));
        
        var aiOut = new Ai_M()
        {
            Id = value.EntityId,
            Location = value.FromLocation,
            LastUpdate = value.LastUpdate
        };
        producerA.Verify(x =>
                x.Produce(KafkaTopic.Ai, key, aiOut),
            Times.Exactly(1)); // keep ai alive (extra event)

        value.ToLocation = new Coordinates_M() {X = 200, Y = 200};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(2));

        value.ToLocation = new Coordinates_M() {X = 300, Y = 300};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(3));
    }

    [Test]
    public void TestProjectileCollisions()
    {
        var key = "test";
        var value = new Collision_M()
        {
            EntityId = key,
            EntityType = EntityType.Bullet,
            FromLocation = new() {X = 0, Y = 0},
            ToLocation = new() {X = 10, Y = 10},
            Direction = new() {X = 10, Y = 10},
            LastUpdate = DateTimeOffset.UtcNow.AddSeconds(-1).Ticks,
            EventId = key,
            TTL = 100
        };

        service.ProcessMessage(key, value);

        var msgOut = new World_M()
        {
            EntityId = value.EntityId,
            Location = value.ToLocation,
            Change = Change.MoveBullet,
            Direction = value.Direction,
            LastUpdate = value.LastUpdate,
            TTL = value.TTL
        };

        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, msgOut),
            Times.Exactly(1));

        value.ToLocation = new Coordinates_M() {X = 100, Y = 100};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(2));
        
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, It.IsAny<string>(), It.IsAny<World_M>()),
            Times.Exactly(3)); // damage agent (extra event)

        value.ToLocation = new Coordinates_M() {X = 200, Y = 200};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(3));

        value.ToLocation = new Coordinates_M() {X = 300, Y = 300};
        service.ProcessMessage(key, value);
        producerW.Verify(x =>
                x.Produce(KafkaTopic.World, key, It.IsAny<World_M>()),
            Times.Exactly(4));
    }
}