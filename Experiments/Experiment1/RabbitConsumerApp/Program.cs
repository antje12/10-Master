using ClassLibrary.Interfaces;
using ClassLibrary.RabbitMQ;

namespace RabbitConsumerApp;

class Program
{
    private static CancellationTokenSource _cts;

    private static RabbitProducer _rp;
    private static RabbitConsumer _rc;

    static async Task Main()
    {
        Setup();
        await RabbitRun();
    }

    private static void Setup()
    {
        _cts = new CancellationTokenSource();
        
        _rp = new RabbitProducer("rabbit-service", "output");
        _rc = new RabbitConsumer("rabbit-service", "input");
    }
    
    private static async Task RabbitRun()
    {
        Console.WriteLine("RabbitMQ Consumer Started");
        IConsumer.ProcessMessage action = SendRabbitResponse;
        await _rc.Consume("input", action, _cts.Token);
    }

    private static void SendRabbitResponse(string key, string value)
    {
        _rp.Produce("output", key, value);
    }
}