// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using Coordinates = ClassLibrary.Classes.Data.Coordinates;
using Input = ClassLibrary.Messages.Avro.Input;
using CollisionCheck = ClassLibrary.Messages.Avro.CollisionCheck;

Console.WriteLine("Hello, World!");

await TestKafkaJson();
//await TestKafkaAvro();
//await TestKafkaProto();

async Task TestKafkaJson()
{
    var testId = Guid.NewGuid();
    var testTopic = KafkaTopic.Collision; //$"{KafkaTopic.LocalState}_{testId}";

    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test", true);
    var admin = new KafkaAdministrator(config);

    await admin.CreateTopic(KafkaTopic.Input);
    await admin.CreateTopic(testTopic);

    var producer = new JsonKafkaProducer<ClassLibrary.Messages.Json.Input>(config);
    var consumer = new JsonKafkaConsumer<ClassLibrary.Messages.Json.CollisionCheck>(config);

    var count = 0;
    var testCount = 1000;
    var results = new List<long>();

    var message = new ClassLibrary.Messages.Json.Input()
    {
        AgentId = testId.ToString(),
        AgentLocation = new ClassLibrary.Messages.Json.Coordinates() {X = 0, Y = 0},
        KeyInput = new List<GameKey>() {GameKey.Right},
        GameTime = 0.5
    };

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, ClassLibrary.Messages.Json.CollisionCheck value)
    {
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //Console.WriteLine($"Kafka result in {elapsedTime} ms");

        if (count > 0)
        {
            results.Add(elapsedTime);
        }

        if (count >= testCount)
        {
            Console.WriteLine(
                $"Kafka results {results.Count}, avg {results.Average()} ms, min {results.Min()} ms, max {results.Max()} ms");
            cts.Cancel();
            return;
        }

        message.AgentLocation = value.ToLocation;

        count += 1;
        stopwatch.Restart();
        producer.Produce(KafkaTopic.Input, "a", message);
    }

    stopwatch.Restart();
    producer.Produce(KafkaTopic.Input, "tester", message);

    IJsonConsumer<ClassLibrary.Messages.Json.CollisionCheck>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}

async Task TestKafkaAvro()
{
    var testId = Guid.NewGuid();
    var testTopic = KafkaTopic.Collision; //$"{KafkaTopic.LocalState}_{testId}";

    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test", true);
    var admin = new KafkaAdministrator(config);

    await admin.CreateTopic(KafkaTopic.Input);
    await admin.CreateTopic(testTopic);

    var producer = new AvroKafkaProducer<Input>(config);
    var consumer = new AvroKafkaConsumer<CollisionCheck>(config);

    var count = 0;
    var testCount = 1000;
    var results = new List<long>();

    var message = new Input()
    {
        AgentId = testId.ToString(),
        AgentLocation = new Coordinates() {X = 0, Y = 0},
        KeyInput = new List<GameKey>() {GameKey.Right},
        GameTime = 0.5
    };

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, CollisionCheck value)
    {
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //Console.WriteLine($"Kafka result in {elapsedTime} ms");

        if (count > 0)
        {
            results.Add(elapsedTime);
        }

        if (count >= testCount)
        {
            Console.WriteLine(
                $"Kafka results {results.Count}, avg {results.Average()} ms, min {results.Min()} ms, max {results.Max()} ms");
            cts.Cancel();
            return;
        }

        message.AgentLocation = value.ToLocation;

        count += 1;
        stopwatch.Restart();
        producer.Produce(KafkaTopic.Input, "a", message);
    }

    stopwatch.Restart();
    producer.Produce(KafkaTopic.Input, "tester", message);

    IAvroConsumer<CollisionCheck>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}

async Task TestKafkaProto()
{
    var testId = Guid.NewGuid();
    var testTopic = KafkaTopic.Collision; //$"{KafkaTopic.LocalState}_{testId}";

    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test", true);
    var admin = new KafkaAdministrator(config);

    await admin.CreateTopic(KafkaTopic.Input);
    await admin.CreateTopic(testTopic);

    var producer = new ProtoKafkaProducer<ClassLibrary.Messages.Protobuf.Input>(config);
    var consumer = new ProtoKafkaConsumer<ClassLibrary.Messages.Protobuf.CollisionCheck>(config);

    var count = 0;
    var testCount = 1000;
    var results = new List<long>();

    var message = new ClassLibrary.Messages.Protobuf.Input()
    {
        AgentId = testId.ToString(),
        AgentLocation = new ClassLibrary.Messages.Protobuf.Coordinates() {X = 0, Y = 0},
        GameTime = 0.5
    };
    message.KeyInput.Add(GameKey.Right);

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, ClassLibrary.Messages.Protobuf.CollisionCheck value)
    {
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        //Console.WriteLine($"Kafka result in {elapsedTime} ms");

        if (count > 0)
        {
            results.Add(elapsedTime);
        }

        if (count >= testCount)
        {
            Console.WriteLine(
                $"Kafka results {results.Count}, avg {results.Average()} ms, min {results.Min()} ms, max {results.Max()} ms");
            cts.Cancel();
            return;
        }

        message.AgentLocation = value.ToLocation;

        count += 1;
        stopwatch.Restart();
        producer.Produce(KafkaTopic.Input, "test", message);
    }

    stopwatch.Restart();
    producer.Produce(KafkaTopic.Input, "init", message);

    IProtoConsumer<ClassLibrary.Messages.Protobuf.CollisionCheck>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}