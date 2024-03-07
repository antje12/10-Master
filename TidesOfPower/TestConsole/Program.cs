// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Avro;
using ClassLibrary.MongoDB;

Console.WriteLine("Hello, World!");
var mongoBroker = new MongoDbBroker();

//TestMongoDB();
//await TestHTTP();
for (int i = 0; i < 10; i++)
{
    //await TestKafkaAvro();
}
for (int i = 0; i < 10; i++)
{
    await TestKafkaProto();
}

void TestMongoDB()
{
    var profile = new Profile()
    {
        Id = Guid.NewGuid(),
        Email = "mail@live.dk",
        Password = "secret"
    };

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    mongoBroker.Insert(profile);
    stopwatch.Stop();
    var elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Create called in {elapsed_time} ms");

    stopwatch.Restart();
    var test = mongoBroker.GetProfile(profile.Id);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Read called in {elapsed_time} ms");

    profile.Id = Guid.NewGuid();

    stopwatch.Restart();
    mongoBroker.Insert(profile);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Create called in {elapsed_time} ms");

    stopwatch.Restart();
    test = mongoBroker.GetProfile(profile.Id);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Read called in {elapsed_time} ms");

    var res = profile.Id == test.Id && profile.Email == test.Email && profile.Password == test.Password;
    Console.WriteLine($"Mongo test result {res}");
}

async Task TestHTTP()
{
    string uri = "http://localhost:5051/InputService/Version";

    HttpClientHandler clientHandler = new HttpClientHandler();
    clientHandler.ServerCertificateCustomValidationCallback =
        (sender, cert, chain, sslPolicyErrors) => { return true; };
    HttpClient client = new HttpClient(clientHandler);

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    using var httpResponse = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

    httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299
    try
    {
        var result = httpResponse.Content.ReadAsStringAsync().Result;
        stopwatch.Stop();
        var elapsed_time = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"HTTP result in {elapsed_time} ms");
    }
    catch // Could be ArgumentNullException or UnsupportedMediaTypeException
    {
        Console.WriteLine("HTTP Response was invalid or could not be deserialized.");
    }
}

async Task TestKafkaAvro()
{
    mongoBroker.CleanDB();
    var testId = Guid.NewGuid();
    var testTopic = $"{KafkaTopic.LocalState}_{testId}";

    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test", true);
    var admin = new KafkaAdministrator(config);

    await admin.CreateTopic(KafkaTopic.Input);
    await admin.CreateTopic(testTopic);

    var producer = new KafkaProducer<Input>(config);
    var consumer = new KafkaConsumer<LocalState>(config);

    var count = 0;
    var testCount = 1000;
    var results = new List<long>();

    var message = new Input()
    {
        PlayerId = testId,
        PlayerLocation = new Coordinates() {X = 0, Y = 0},
        KeyInput = new List<GameKey>() {GameKey.Right},
        GameTime = 0.5
    };

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, LocalState value)
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

        message.PlayerLocation = value.Avatars.First().Location;

        count += 1;
        stopwatch.Restart();
        producer.Produce(KafkaTopic.Input, "a", message);
    }

    stopwatch.Restart();
    producer.Produce(KafkaTopic.Input, "tester", message);

    IConsumer<LocalState>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}

async Task TestKafkaProto()
{
    mongoBroker.CleanDB();
    var testId = Guid.NewGuid();
    var testTopic = $"{KafkaTopic.LocalState}_{testId}";

    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test", true);
    var admin = new KafkaAdministrator(config);

    await admin.CreateTopic(KafkaTopic.Input);
    await admin.CreateTopic(testTopic);

    var producer = new ProtoKafkaProducer<ClassLibrary.Messages.Protobuf.Input>(config);
    var consumer = new ProtoKafkaConsumer<ClassLibrary.Messages.Protobuf.LocalState>(config);

    var count = 0;
    var testCount = 1000;
    var results = new List<long>();

    var message = new ClassLibrary.Messages.Protobuf.Input()
    {
        PlayerId = testId.ToString(),
        PlayerLocation = new ClassLibrary.Messages.Protobuf.Coordinates() {X = 0, Y = 0},
        GameTime = 0.5
    };
    message.KeyInput.Add(ClassLibrary.Messages.Protobuf.GameKey.Right);

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, ClassLibrary.Messages.Protobuf.LocalState value)
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

        message.PlayerLocation = value.Avatars.First().Location;

        count += 1;
        stopwatch.Restart();
        producer.Produce(KafkaTopic.Input, "test", message);
    }

    stopwatch.Restart();
    producer.Produce(KafkaTopic.Input, "init", message);

    IProtoConsumer<ClassLibrary.Messages.Protobuf.LocalState>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}