namespace TestConsole.Tests;

public class Latency
{
    public async Task Test()
    {
        var tasks = new List<Task>();
        var numberOfClients = 20;

        for (int i = 0; i < numberOfClients; i++)
        {
            var client = new LatencyClient(i);
            tasks.Add(Task.Run(() => client.RunLatencyTest()));
        }

        await Task.WhenAll(tasks);
    }
}