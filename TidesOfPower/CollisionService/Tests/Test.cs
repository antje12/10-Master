using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;
using NUnit.Framework;
using Moq;

namespace CollisionService.Tests;

[TestFixture]
public class Test
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
    }
    
    [OneTimeTearDown]
    public void OneTimeCleanup()
    {
    }
    
    [SetUp]
    public void Setup()
    {
    }

    [TearDown]
    public void Cleanup()
    {
    }
    
    [Test]
    public async Task TestExecuteAsync()
    {
        var admin = new Mock<IAdministrator>();
        admin.Setup(x => x.CreateTopic(It.IsAny<KafkaTopic>()))
            .Returns(Task.CompletedTask);

        var consumer = new Mock<IProtoConsumer<Collision_M>>();
        consumer.Setup(x => x.Consume(
                It.IsAny<KafkaTopic>(),
                It.IsAny<IProtoConsumer<Collision_M>.ProcessMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () => { await Task.Delay(500); });

        var producerW = new Mock<IProtoProducer<World_M>>();
        var producerA = new Mock<IProtoProducer<Ai_M>>();
        
        var redis = new Mock<RedisBroker>(false);
        redis.Setup(x => x.Connect());

        var service = new Services.CollisionService();
        service.Admin = admin.Object;
        service.ProducerW = producerW.Object;
        service.ProducerA = producerA.Object;
        service.Consumer = consumer.Object;
        service.RedisBroker = redis.Object;

        var executeTask =  service.ExecuteAsync();
        await Task.Delay(100);
        Assert.That(service.IsRunning, Is.True);
        await executeTask;

        admin.Verify(x => x.CreateTopic(KafkaTopic.Collision));
        consumer.Verify(x => x.Consume(
            KafkaTopic.Collision,
            It.IsAny<IProtoConsumer<Collision_M>.ProcessMessage>(),
            It.IsAny<CancellationToken>()));
    }
}