// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using ClassLibrary.Classes;
using ClassLibrary.Classes.Client;
using ClassLibrary.Interfaces;
using ClassLibrary.Kafka;

Console.WriteLine("Hello, World!");

string uri = "http://localhost:5051/InputService/Version";

HttpClientHandler clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
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

var cts = new CancellationTokenSource();
var config= new KafkaConfig("test");
var admin= new KafkaAdministrator(config);

admin.CreateTopic(KafkaTopic.Input);
admin.CreateTopic(KafkaTopic.LocalState);

var producer= new KafkaProducer<Input>(config);
var consumer= new KafkaConsumer<Output>(config);

var first = true;

void ProcessMessage(string key, Output value)
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
        Timer = 0.5
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
    Timer = 0.5
});

IConsumer<Output>.ProcessMessage action = ProcessMessage;
await Task.Run(() => consumer.Consume(KafkaTopic.LocalState, action, cts.Token), cts.Token);
