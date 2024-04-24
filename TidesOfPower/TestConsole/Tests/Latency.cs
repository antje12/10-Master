namespace TestConsole.Tests;

public class Latency
{
    public async Task Test()
    {
        var tasks = new List<Task>();
        var numberOfClients = 10;

        for (int i = 0; i < numberOfClients; i++)
        {
            var client = new KafkaLatencyClient(i);
            tasks.Add(Task.Run(() => client.RunLatencyTest()));
        }

        await Task.WhenAll(tasks);
    }
}