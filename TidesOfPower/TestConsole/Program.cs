using System.Diagnostics;
using ClassLibrary.Domain;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.MongoDB;
using ClassLibrary.Redis;
using TestConsole.Tests;

Console.WriteLine("Hello, World!");
RedisBroker redisBroker = new RedisBroker();
redisBroker.Connect(true);
redisBroker.Clean();

var latency = new Latency();
await latency.Test();
return;


MongoDbBroker mongoBroker = new MongoDbBroker();
mongoBroker.Connect(true);

//redisBroker.InitEntity();
//var agent = new Agent(
//    Guid.NewGuid(),
//    new Coordinates(50.123f, 100.456f),
//    EntityType.Player, 100, 100);
//redisBroker.Insert(agent);
//redisBroker.GetEntities(agent.Location.X, agent.Location.Y);

//redisBroker.InitProfile();
//TestRedis();
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
    var profile = new Profile(
        Guid.NewGuid(),
        "mail@live.dk",
        "secret");

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    //mongoBroker.Insert(profile);
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

void TestRedis()
{
    var profile = new Profile(
        Guid.NewGuid(),
        "mail@live.dk",
        "secret");

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    redisBroker.Insert(profile);
    stopwatch.Stop();
    var elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Create called in {elapsed_time} ms");

    stopwatch.Restart();
    var test = redisBroker.GetProfiles(profile.Id);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Read called in {elapsed_time} ms");

    profile.Id = Guid.NewGuid();

    stopwatch.Restart();
    redisBroker.Insert(profile);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Create called in {elapsed_time} ms");

    stopwatch.Restart();
    test = redisBroker.GetProfiles(profile.Id);
    stopwatch.Stop();
    elapsed_time = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Read called in {elapsed_time} ms");

    var res = profile.Id == test.Id && profile.Email == test.Email && profile.Password == test.Password;
    Console.WriteLine($"Redis test result {res}");
}

async Task TestHTTP()
{
    string uri = "http://localhost:8080/RestService/Version";

    HttpClientHandler clientHandler = new HttpClientHandler();
    clientHandler.ServerCertificateCustomValidationCallback =
        (sender, cert, chain, sslPolicyErrors) => { return true; };
    HttpClient client = new HttpClient(clientHandler);

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    await HttpTest(client, uri, stopwatch);
}

async Task HttpTest(HttpClient httpClient, string s, Stopwatch stopwatch)
{
    stopwatch.Restart();
    using var httpResponse = await httpClient.GetAsync(s, HttpCompletionOption.ResponseHeadersRead);
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

async Task TestKafkaProto()
{
    //redisBroker.Clean();
    var testId = Guid.NewGuid();
    var testTopic = KafkaTopic.Collision; //$"{KafkaTopic.LocalState}_{testId}";

    var cts = new CancellationTokenSource();
    var config = new KafkaConfig("test", true);
    var admin = new KafkaAdministrator(config);

    await admin.CreateTopic(KafkaTopic.Input);
    await admin.CreateTopic(testTopic);

    var producer = new KafkaProducer<Input_M>(config);
    var consumer = new KafkaConsumer<Collision_M>(config);

    var count = 0;
    var testCount = 100;
    var results = new List<long>();

    var message = new Input_M
    {
        AgentId = testId.ToString(),
        AgentLocation = new Coordinates_M {X = 0, Y = 0},
        GameTime = 0.5
    };
    message.KeyInput.Add(GameKey.Right);

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    void ProcessMessage(string key, Collision_M value)
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
        //.First(x => x.Id == testId.ToString()).Location;

        count += 1;
        stopwatch.Restart();
        producer.Produce(KafkaTopic.Input, "test", message);
    }

    stopwatch.Restart();
    producer.Produce(KafkaTopic.Input, "init", message);

    IProtoConsumer<Collision_M>.ProcessMessage action = ProcessMessage;
    await Task.Run(() => consumer.Consume(testTopic, action, cts.Token), cts.Token);
}