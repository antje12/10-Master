// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Classes.Messages;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;

Console.WriteLine("Hello, World!");

TestMongoDB();
await TestHTTP();
await TestKafka();

void TestMongoDB()
{
    var mongoBroker = new MongoDbBroker();

    var profile = new Profile()
    {
        Id = Guid.NewGuid(),
        Email = "mail@live.dk",
        Password = "secret"
    };

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    mongoBroker.Create(profile);
    stopwatch.Stop();
    var elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Create called in {elapsed_time} ms");

    stopwatch.Restart();
    var test = mongoBroker.Read(profile.Id);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Read called in {elapsed_time} ms");

    profile.Id = Guid.NewGuid();

    stopwatch.Restart();
    mongoBroker.Create(profile);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Create called in {elapsed_time} ms");

    stopwatch.Restart();
    test = mongoBroker.Read(profile.Id);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Read called in {elapsed_time} ms");

    var res = profile.Id == test.Id && profile.Email == test.Email && profile.Password == test.Password;
    Console.WriteLine($"Test result {res}");
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

async Task TestKafka()
{
    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test");
    var admin = new KafkaAdministrator(config);

    admin.CreateTopic(KafkaTopic.Input);
    admin.CreateTopic(KafkaTopic.LocalState);

    var producer = new KafkaProducer<Input>(config);
    var consumer = new KafkaConsumer<LocalState>(config);

    var first = true;

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    void ProcessMessage(string key, LocalState value)
    {
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Kafka result in {elapsedTime} ms");

        if (!first)
            return;

        first = false;
        stopwatch.Restart();
        producer.Produce(KafkaTopic.Input, "a", new Input()
        {
            PlayerId = Guid.NewGuid(),
            Location = new Coordinates()
            {
                X = 0,
                Y = 0
            },
            KeyInput = new List<GameKey>() {GameKey.Right},
            GameTime = 0.5
        });
    }

    stopwatch.Restart();
    producer.Produce(KafkaTopic.Input, "tester", new Input()
    {
        PlayerId = Guid.NewGuid(),
        Location = new Coordinates()
        {
            X = 0,
            Y = 0
        },
        KeyInput = new List<GameKey>() {GameKey.Right},
        GameTime = 0.5
    });

    IConsumer<LocalState>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(KafkaTopic.LocalState, action, cts.Token), cts.Token);
}
