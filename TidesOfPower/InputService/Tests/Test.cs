using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using NUnit.Framework;
using Moq;

namespace InputService.Tests;

[TestFixture]
public class Test : Services.InputService
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

        var consumer = new Mock<IProtoConsumer<Input_M>>();
        consumer.Setup(x => x.Consume(
                It.IsAny<KafkaTopic>(),
                It.IsAny<IProtoConsumer<Input_M>.ProcessMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () => { await Task.Delay(500); });

        var producerC = new Mock<IProtoProducer<Collision_M>>();
        var producerW = new Mock<IProtoProducer<World_M>>();

        var service = new Services.InputService();
        service.Admin = admin.Object;
        service.ProducerC = producerC.Object;
        service.ProducerW = producerW.Object;
        service.Consumer = consumer.Object;

        var executeTask =  service.ExecuteAsync();
        await Task.Delay(100);
        Assert.That(service.IsRunning, Is.True);
        await executeTask;

        admin.Verify(x => x.CreateTopic(KafkaTopic.Input));
        consumer.Verify(x => x.Consume(
            KafkaTopic.Input,
            It.IsAny<IProtoConsumer<Input_M>.ProcessMessage>(),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public void TestProcessMessage()
    {
        var admin = new Mock<KafkaAdministrator>();
        var producerC = new Mock<KafkaProducer<Collision_M>>();
        var producerW = new Mock<KafkaProducer<World_M>>();
        var consumer = new Mock<KafkaConsumer<Input_M>>();

        var service = new Services.InputService();
        service.Admin = admin.Object;
        service.ProducerC = producerC.Object;
        service.ProducerW = producerW.Object;
        service.Consumer = consumer.Object;

        var key = "";
        var value = new Input_M()
        {
        };
        service.ProcessMessage(key, value);
    }
}