using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using NUnit.Framework;
using Moq;

namespace ProjectileService.Tests;

[TestFixture]
public class Test
{
    private Mock<IAdministrator> admin;
    private Mock<IProtoConsumer<Projectile_M>> consumer;
    private Mock<IProtoProducer<Collision_M>> producer;
    private Services.ProjectileService service;

    [SetUp]
    public void Setup()
    {
        admin = new Mock<IAdministrator>();
        admin.Setup(x => x.CreateTopic(It.IsAny<KafkaTopic>()))
            .Returns(Task.CompletedTask);

        consumer = new Mock<IProtoConsumer<Projectile_M>>();
        consumer.Setup(x => x.Consume(
                It.IsAny<KafkaTopic>(),
                It.IsAny<IProtoConsumer<Projectile_M>.ProcessMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () => { await Task.Delay(500); });

        producer = new Mock<IProtoProducer<Collision_M>>();
        producer.Setup(x => x.Produce(
            It.IsAny<KafkaTopic>(),
            It.IsAny<string>(),
            It.IsAny<Collision_M>()));

        service = new Services.ProjectileService();
        service.Admin = admin.Object;
        service.Consumer = consumer.Object;
        service.Producer = producer.Object;
    }

    [Test]
    public async Task TestExecuteAsync()
    {
        var executeTask = service.ExecuteAsync();
        await Task.Delay(100);
        Assert.That(service.IsRunning, Is.True);
        await executeTask;

        admin.Verify(x => x.CreateTopic(KafkaTopic.Projectile));
        consumer.Verify(x => x.Consume(
            KafkaTopic.Projectile,
            It.IsAny<IProtoConsumer<Projectile_M>.ProcessMessage>(),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public void TestMoveProjectile()
    {
        var key = Guid.NewGuid().ToString();
        var value = new Projectile_M
        {
            Id = key,
            Location = new Coordinates_M {X = 0, Y = 0},
            Direction = new Coordinates_M {X = 10, Y = 10},
            LastUpdate = DateTime.UtcNow.AddSeconds(-1).Ticks,
            TTL = 100
        };
        Collision_M state = null;
        producer.Setup(x => 
                x.Produce(KafkaTopic.Collision, key, It.IsAny<Collision_M>()))
            .Callback<KafkaTopic, string, Collision_M>((topic, entityId, msg) => state = msg);
        service.ProcessMessage(key, value);
        
        Assert.That(state, Is.Not.Null);
        Assert.That(state.FromLocation.X, Is.EqualTo(0f));
        Assert.That(state.FromLocation.Y, Is.EqualTo(0f));
        Assert.That(state.ToLocation.X, Is.Not.EqualTo(0f));
        Assert.That(state.ToLocation.Y, Is.Not.EqualTo(0f));
        
        producer.Verify(x => x.Produce(
                KafkaTopic.Collision,
                key,
                It.IsAny<Collision_M>()),
            Times.Exactly(1));
    }
}