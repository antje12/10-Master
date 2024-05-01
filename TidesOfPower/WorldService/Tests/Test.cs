using ClassLibrary.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.MongoDB;
using ClassLibrary.Redis;
using NUnit.Framework;
using Moq;

namespace WorldService.Tests;

[TestFixture]
public class Test
{
    private Mock<IAdministrator> admin;
    private Mock<IProtoConsumer<World_M>> consumer;
    private Mock<IProtoProducer<LocalState_M>> producerLS;
    private Mock<IProtoProducer<Projectile_M>> producerP;
    private Mock<IProtoProducer<Ai_M>> producerA;
    private Mock<RedisBroker> redis;
    private Mock<MongoDbBroker> mongo;
    private Services.WorldService service;

    [SetUp]
    public void Setup()
    {
        admin = new Mock<IAdministrator>();
        admin.Setup(x => x.CreateTopic(It.IsAny<KafkaTopic>()))
            .Returns(Task.CompletedTask);

        consumer = new Mock<IProtoConsumer<World_M>>();
        consumer.Setup(x => x.Consume(
                It.IsAny<KafkaTopic>(),
                It.IsAny<IProtoConsumer<World_M>.ProcessMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () => { await Task.Delay(500); });

        producerLS = new Mock<IProtoProducer<LocalState_M>>();
        producerLS.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(),
            It.IsAny<string>(),
            It.IsAny<LocalState_M>()));

        producerP = new Mock<IProtoProducer<Projectile_M>>();
        producerP.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(),
            It.IsAny<string>(),
            It.IsAny<Projectile_M>()));

        producerA = new Mock<IProtoProducer<Ai_M>>();
        producerA.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(),
            It.IsAny<string>(),
            It.IsAny<Ai_M>()));

        redis = new Mock<RedisBroker>();
        redis.Setup(x => x.Connect(false));
        redis.Setup(x => x.GetEntities(
                It.IsAny<float>(),
                It.IsAny<float>()))
            .Returns(new List<Entity>()
            {
                new Player("Player", 0, Guid.NewGuid(), new Coordinates(10, 10), 100, 100),
                new Enemy(Guid.NewGuid(), new Coordinates(100, 100), 100, 100),
                new Projectile(new Coordinates(0, 0), 100, 100, 100, Guid.NewGuid(), new Coordinates(200, 200)),
                new Treasure(100, Guid.NewGuid(), new Coordinates(300, 300))
            });
        redis.Setup(x => x.Get(It.IsAny<Guid>()))
            .Returns(new Agent(Guid.NewGuid(), new Coordinates(0, 0), ClassLibrary.Domain.EntityType.Player, 100, 100));
        redis.Setup(x => x.UpsertAgentLocation(It.IsAny<Agent>()));
        redis.Setup(x => x.DeleteEntity(It.IsAny<Guid>()));
        redis.Setup(x => x.Insert(It.IsAny<Entity>()));
        
        mongo = new Mock<MongoDbBroker>();       
        mongo.Setup(x => x.Connect(false));
        
        service = new Services.WorldService();
        service.Admin = admin.Object;
        service.Consumer = consumer.Object;
        service.ProducerLS = producerLS.Object;
        service.ProducerP = producerP.Object;
        service.ProducerA = producerA.Object;
        service.RedisBroker = redis.Object;
        //service.MongoBroker = mongo.Object;
    }

    [Test]
    public async Task TestExecuteAsync()
    {
        var executeTask = service.ExecuteAsync();
        await Task.Delay(100);
        Assert.That(service.IsRunning, Is.True);
        await executeTask;

        admin.Verify(x => x.CreateTopic(KafkaTopic.World));
        consumer.Verify(x => x.Consume(
            KafkaTopic.World,
            It.IsAny<IProtoConsumer<World_M>.ProcessMessage>(),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public void TestMovePlayer()
    {
        var key = Guid.NewGuid().ToString();
        var value = new World_M()
        {
            EntityId = key,
            Location = new Coordinates_M {X = 0, Y = 0},
            Change = Change.MovePlayer,
            EventId = key,
            Value = 100
        };
        
        Player? player = null;
        redis.Setup(x => 
                x.UpsertAgentLocation(It.IsAny<Agent>()))
            .Callback<Agent>((agent) => player = (Player) agent);
        LocalState_M? state = null;
        producerLS.Setup(x => 
                x.Produce(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LocalState_M>()))
            .Callback<string, string, LocalState_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);

        redis.Verify(x => x.Get(Guid.Parse(value.EntityId)));
        redis.Verify(x => x.UpsertAgentLocation(It.IsAny<Agent>()));
        
        Assert.That(player, Is.Not.Null);
        Assert.That(player.Name, Is.EqualTo("Player"));
        Assert.That(player.Score, Is.EqualTo(100));
        Assert.That(player.Id, Is.EqualTo(Guid.Parse(key)));
        Assert.That(player.Location.X, Is.EqualTo(0));
        Assert.That(player.Location.Y, Is.EqualTo(0));
        Assert.That(player.LifePool, Is.EqualTo(100));
        
        redis.Verify(x => x.GetEntities(0, 0));
        producerLS.Verify(x => x.Produce(
                $"{KafkaTopic.LocalState}_{key}", key,
                It.IsAny<LocalState_M>()),
            Times.Exactly(1));
        producerLS.Verify(x => x.Produce(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<LocalState_M>()),
            Times.Exactly(2));
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Sync, Is.EqualTo(Sync.Delta));
    }

    [Test]
    public void TestMoveAi()
    {
        var key = Guid.NewGuid().ToString();
        var value = new World_M()
        {
            EntityId = key,
            Location = new Coordinates_M {X = 0, Y = 0},
            Change = Change.MoveAi,
            LastUpdate = DateTime.UtcNow.AddSeconds(-1).Ticks
        };
        
        Enemy? enemy = null;
        redis.Setup(x => 
                x.UpsertAgentLocation(It.IsAny<Agent>()))
            .Callback<Agent>((agent) => enemy = (Enemy) agent);
        LocalState_M? state = null;
        producerLS.Setup(x => 
                x.Produce(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LocalState_M>()))
            .Callback<string, string, LocalState_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);

        redis.Verify(x => x.Get(Guid.Parse(value.EntityId)));
        redis.Verify(x => x.UpsertAgentLocation(It.IsAny<Agent>()));
        
        Assert.That(enemy, Is.Not.Null);
        Assert.That(enemy.Id, Is.EqualTo(Guid.Parse(key)));
        Assert.That(enemy.Location.X, Is.EqualTo(0));
        Assert.That(enemy.Location.Y, Is.EqualTo(0));
        Assert.That(enemy.LifePool, Is.EqualTo(100));
        
        var msgOut = new Ai_M
        {
            Id = value.EntityId,
            Location = value.Location,
            LastUpdate = value.LastUpdate
        };
        producerA.Verify(x => x.Produce(
                KafkaTopic.Ai, key,
                msgOut),
            Times.Exactly(1));
        
        redis.Verify(x => x.GetEntities(0, 0));
        producerLS.Verify(x => x.Produce(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<LocalState_M>()),
            Times.Exactly(1));
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Sync, Is.EqualTo(Sync.Delta));
    }

    [Test]
    public void TestMoveBullet()
    {
        var key = Guid.NewGuid().ToString();
        var value = new World_M()
        {
            EntityId = key,
            Location = new Coordinates_M {X = 0, Y = 0},
            Direction = new Coordinates_M {X = 10, Y = 10},
            Change = Change.MoveBullet,
            LastUpdate = DateTime.UtcNow.AddSeconds(-1).Ticks,
            TTL = 100
        };
        
        LocalState_M? state = null;
        producerLS.Setup(x => 
                x.Produce(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LocalState_M>()))
            .Callback<string, string, LocalState_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);
        
        var msgOut = new Projectile_M
        {
            Id = value.EntityId,
            Location = value.Location,
            Direction = value.Direction,
            LastUpdate = value.LastUpdate,
            TTL = value.TTL
        };
        producerP.Verify(x => x.Produce(
                KafkaTopic.Projectile, msgOut.Id, msgOut),
            Times.Exactly(1));
        
        producerLS.Verify(x => x.Produce(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<LocalState_M>()),
            Times.Exactly(1));
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Sync, Is.EqualTo(Sync.Delta));

        value.TTL = 0;
        service.ProcessMessage(key, value);
        producerP.Verify(x => x.Produce(
                It.IsAny<KafkaTopic>(), It.IsAny<string>(),
                It.IsAny<Projectile_M>()),
            Times.Exactly(1));
        
        producerLS.Verify(x => x.Produce(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<LocalState_M>()),
            Times.Exactly(2));
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Sync, Is.EqualTo(Sync.Delete));
    }

    [Test]
    public void TestSpawnAi()
    {
        var key = Guid.NewGuid().ToString();
        var value = new World_M()
        {
            Location = new Coordinates_M {X = 0, Y = 0},
            Change = Change.SpawnAi
        };
        
        Enemy? enemy = null;
        redis.Setup(x => 
                x.UpsertAgentLocation(It.IsAny<Agent>()))
            .Callback<Agent>((agent) => enemy = (Enemy) agent);
        LocalState_M? state = null;
        producerLS.Setup(x => 
                x.Produce(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LocalState_M>()))
            .Callback<string, string, LocalState_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);
        
        redis.Verify(x => x.UpsertAgentLocation(It.IsAny<Agent>()));
        
        Assert.That(enemy, Is.Not.Null);
        Assert.That(enemy.Location.X, Is.EqualTo(0));
        Assert.That(enemy.Location.Y, Is.EqualTo(0));
        Assert.That(enemy.LifePool, Is.EqualTo(100));
        
        producerA.Verify(x => x.Produce(
                KafkaTopic.Ai, It.IsAny<string>(), It.IsAny<Ai_M>()),
            Times.Exactly(1));
        
        redis.Verify(x => x.GetEntities(0, 0));
        producerLS.Verify(x => x.Produce(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<LocalState_M>()),
            Times.Exactly(1));
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Sync, Is.EqualTo(Sync.Delta));
    }

    [Test]
    public void TestSpawnBullet()
    {
        var key = Guid.NewGuid().ToString();
        var value = new World_M()
        {
            EntityId = key,
            Location = new Coordinates_M {X = 0, Y = 0},
            Direction = new Coordinates_M {X = 10, Y = 10},
            Change = Change.SpawnBullet
        };
        
        LocalState_M? state = null;
        producerLS.Setup(x => 
                x.Produce(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LocalState_M>()))
            .Callback<string, string, LocalState_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);
        
        producerP.Verify(x => x.Produce(
                KafkaTopic.Projectile, It.IsAny<string>(),
                It.IsAny<Projectile_M>()),
            Times.Exactly(1));
        
        producerLS.Verify(x => x.Produce(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<LocalState_M>()),
            Times.Exactly(1));
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Sync, Is.EqualTo(Sync.Delta));
    }

    [Test]
    public void TestDamageAgent()
    {
        var key = Guid.NewGuid().ToString();
        var value = new World_M()
        {
            EntityId = key,
            Location = new Coordinates_M {X = 0, Y = 0},
            Change = Change.DamageAgent
        };
        
        LocalState_M? state = null;
        producerLS.Setup(x => 
                x.Produce(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LocalState_M>()))
            .Callback<string, string, LocalState_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);

        redis.Verify(x => x.Get(Guid.Parse(value.EntityId)));
        
        redis.Verify(x => x.Insert(It.IsAny<Treasure>()));
        
        redis.Verify(x => x.GetEntities(0, 0));
        redis.Verify(x => x.DeleteEntity(Guid.Parse(key)));
        
        producerLS.Verify(x => x.Produce(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<LocalState_M>()),
            Times.Exactly(2));
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Sync, Is.EqualTo(Sync.Delete));
    }

    [Test]
    public void TestCollectTreasure()
    {
        var key = Guid.NewGuid().ToString();
        var value = new World_M()
        {
            EntityId = key,
            Value = 100,
            Location = new Coordinates_M {X = 0, Y = 0},
            Change = Change.CollectTreasure
        };
        
        LocalState_M? state = null;
        producerLS.Setup(x => 
                x.Produce(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LocalState_M>()))
            .Callback<string, string, LocalState_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);
        
        redis.Verify(x => x.GetEntities(0, 0));
        redis.Verify(x => x.DeleteEntity(Guid.Parse(key)));
        
        producerLS.Verify(x => x.Produce(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<LocalState_M>()),
            Times.Exactly(1));
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Sync, Is.EqualTo(Sync.Delete));
    }
}