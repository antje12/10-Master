// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ClassLibrary.Avro;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Classes.Messages;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.MongoDB;

Console.WriteLine("Hello, World!");

//TestMongoDB();
//await TestHTTP();
//await TestKafka();
await TestSimpleKafka();
await TestProtoKafka();

void TestMongoDB()
{
    Console.WriteLine("TestMongoDB");
    var mongoBroker = new MongoDbBroker();

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
    Console.WriteLine($"Test result {res}");
}

async Task TestHTTP()
{
    Console.WriteLine("TestHTTP");
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
    Console.WriteLine("TestKafka");
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
    var testCount = 10;

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, LocalState value)
    {
        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Kafka result in {elapsedTime} ms");

        if (count >= testCount)
        {
            cts.Cancel();
            return;
        }

        count +=1;
        stopwatch.Restart();
        producer.Produce(KafkaTopic.Input, "a", new Input()
        {
            PlayerId = testId,
            PlayerLocation = new Coordinates()
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
        PlayerId = testId,
        PlayerLocation = new Coordinates()
        {
            X = 0,
            Y = 0
        },
        KeyInput = new List<GameKey>() {GameKey.Right},
        GameTime = 0.5
    });

    IConsumer<LocalState>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}

async Task TestSimpleKafka()
{
    Console.WriteLine("TestSimpleKafka");
    var testTopic = "simple_test";

    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test", true);
    var admin = new KafkaAdministrator(config);

    await admin.CreateTopic(testTopic);

    var producer = new KafkaProducer<AvroUser>(config);
    var consumer = new KafkaConsumer<AvroUser>(config);

    var count = 0;
    var testCount = 1000;
    var results = new List<long>();

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, AvroUser value)
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
            Console.WriteLine($"Kafka results {results.Count}, avg {results.Average()} ms");
            cts.Cancel();
            return;
        }

        count +=1;
        stopwatch.Restart();
        producer.Produce(testTopic, "a", new AvroUser()
        {
            Name = "Jack",
            FavoriteNumber = 22,
            FavoriteColor = "Green"
        });
    }

    stopwatch.Restart();
    producer.Produce(testTopic, "a", new AvroUser()
    {
        Name = "Jack",
        FavoriteNumber = 22,
        FavoriteColor = "Green"
    });

    IConsumer<AvroUser>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}

async Task TestProtoKafka()
{
    Console.WriteLine("TestProtoKafka");
    var testTopic = "proto_test";

    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test", true);
    var admin = new KafkaAdministrator(config);

    await admin.CreateTopic(testTopic);

    var producer = new ProtoKafkaProducer<User>(config);
    var consumer = new ProtoKafkaConsumer<User>(config);

    var count = 0;
    var testCount = 1000;
    var results = new List<long>();

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, User value)
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
            Console.WriteLine($"Kafka results {results.Count}, avg {results.Average()} ms");
            cts.Cancel();
            return;
        }

        count +=1;
        stopwatch.Restart();
        producer.Produce(testTopic, "a", new User()
        {
            Name = "Jack",
            FavoriteNumber = 22,
            FavoriteColor = "Green"
        });
    }

    stopwatch.Restart();
    producer.Produce(testTopic, "a", new User()
    {
        Name = "Jack",
        FavoriteNumber = 22,
        FavoriteColor = "Green"
    });

    IProtoConsumer<User>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}