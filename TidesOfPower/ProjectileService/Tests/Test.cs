using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using NUnit.Framework;
using Moq;

namespace ProjectileService.Tests;

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

        var consumer = new Mock<IProtoConsumer<Projectile_M>>();
        consumer.Setup(x => x.Consume(
                It.IsAny<KafkaTopic>(),
                It.IsAny<IProtoConsumer<Projectile_M>.ProcessMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () => { await Task.Delay(500); });
        
        var service = new Services.ProjectileService();
        service.Admin = admin.Object;
        service.Consumer = consumer.Object;

        var executeTask =  service.ExecuteAsync();
        await Task.Delay(100);
        Assert.That(service.IsRunning, Is.True);
        await executeTask;

        admin.Verify(x => x.CreateTopic(KafkaTopic.Projectile));
        consumer.Verify(x => x.Consume(
            KafkaTopic.Projectile,
            It.IsAny<IProtoConsumer<Projectile_M>.ProcessMessage>(),
            It.IsAny<CancellationToken>()));
    }
}