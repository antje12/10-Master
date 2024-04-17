using ClassLibrary.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;
using NUnit.Framework;
using Moq;

namespace AIService.Tests;

[TestFixture]
public class Test
{
    private Mock<IAdministrator> admin;
    private Mock<IProtoConsumer<Ai_M>> consumer;
    private Mock<IProtoProducer<Input_M>> producer;
    private Mock<RedisBroker> redis;
    private Services.AIService service;

    [SetUp]
    public void Setup()
    {
        admin = new Mock<IAdministrator>();
        admin.Setup(x => x.CreateTopic(It.IsAny<KafkaTopic>()))
            .Returns(Task.CompletedTask);

        consumer = new Mock<IProtoConsumer<Ai_M>>();
        consumer.Setup(x => x.Consume(
                It.IsAny<KafkaTopic>(),
                It.IsAny<IProtoConsumer<Ai_M>.ProcessMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () => { await Task.Delay(500); });

        producer = new Mock<IProtoProducer<Input_M>>();
        producer.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(),
            It.IsAny<string>(),
            It.IsAny<Input_M>()));

        redis = new Mock<RedisBroker>();
        redis.Setup(x => x.Connect(false));
        redis.Setup(x => x.GetEntities(It.IsAny<float>(), It.IsAny<float>()))
            .Returns(new List<Entity>()
            {
                new Player("Player", 0, Guid.NewGuid(), new Coordinates(10, 10), 100, 100),
                new Enemy(Guid.NewGuid(), new Coordinates(100, 100), 100, 100),
                new Projectile(new Coordinates(0, 0), 100, 100, 100, Guid.NewGuid(), new Coordinates(200, 200)),
                new Treasure(100, Guid.NewGuid(), new Coordinates(300, 300))
            });
        redis.Setup(x => x.GetEntities(1000, 1000))
            .Returns(new List<Entity>() {});
        redis.Setup(x => x.Get(It.IsAny<Guid>()))
            .Returns(new Agent(Guid.NewGuid(), new Coordinates(0, 0), ClassLibrary.Domain.EntityType.Enemy, 100, 100));

        service = new Services.AIService();
        service.Admin = admin.Object;
        service.Consumer = consumer.Object;
        service.Producer = producer.Object;
        service.RedisBroker = redis.Object;
    }

    [Test]
    public async Task TestExecuteAsync()
    {
        var executeTask = service.ExecuteAsync();
        await Task.Delay(100);
        Assert.That(service.IsRunning, Is.True);
        await executeTask;

        admin.Verify(x => x.CreateTopic(KafkaTopic.Ai));
        consumer.Verify(x => x.Consume(
            KafkaTopic.Ai,
            It.IsAny<IProtoConsumer<Ai_M>.ProcessMessage>(),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public void TestMoveAI()
    {
        var key = Guid.NewGuid().ToString();
        var value = new Ai_M
        {
            Id = key,
            Location = new Coordinates_M {X = 0, Y = 0},
            LastUpdate = DateTime.UtcNow.AddSeconds(-1).Ticks
        };

        Input_M? state = null;
        producer.Setup(x =>
                x.Produce(KafkaTopic.Input, key, It.IsAny<Input_M>()))
            .Callback<KafkaTopic, string, Input_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);

        redis.Verify(x => x.Get(Guid.Parse(key)));
        redis.Verify(x => x.GetEntities(0, 0));

        Assert.That(state, Is.Not.Null);
        Assert.That(state.AgentLocation, Is.EqualTo(value.Location));
        Assert.That(state.MouseLocation, Is.EqualTo(new Coordinates_M(){X=10f,Y=10f}));
        Assert.That(state.KeyInput, Contains.Item(GameKey.Attack));

        producer.Verify(x => x.Produce(
                KafkaTopic.Input,
                key,
                It.IsAny<Input_M>()),
            Times.Exactly(1));

        value.Location = new Coordinates_M() {X = 1000, Y = 1000};
        service.ProcessMessage(key, value);
        Assert.That(state, Is.Not.Null);
        Assert.That(state.AgentLocation, Is.EqualTo(value.Location));
        Assert.That(state.MouseLocation, Is.Null);
        Assert.That(state.KeyInput, Does.Not.Contain(GameKey.Attack));
        Assert.That(state.KeyInput, Contains.Item(GameKey.Up)
            .Or.Contain(GameKey.Down)
            .Or.Contain(GameKey.Right)
            .Or.Contain(GameKey.Left));

        producer.Verify(x => x.Produce(
                KafkaTopic.Input,
                key,
                It.IsAny<Input_M>()),
            Times.Exactly(2));

        value.Location = new Coordinates_M() {X = 140, Y = 140};
        service.ProcessMessage(key, value);
        Assert.That(state, Is.Not.Null);
        Assert.That(state.AgentLocation, Is.EqualTo(value.Location));
        Assert.That(state.MouseLocation, Is.Null);
        Assert.That(state.KeyInput, Does.Not.Contain(GameKey.Attack));
        Assert.That(state.KeyInput, Contains.Item(GameKey.Down));

        producer.Verify(x => x.Produce(
                KafkaTopic.Input,
                key,
                It.IsAny<Input_M>()),
            Times.Exactly(3));
    }
}