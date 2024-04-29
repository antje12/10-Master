namespace TestConsole.Tests;

public class Latency
{
    public async Task Test()
    {
        var tasks = new List<Task>();
        var numberOfClients = 10;
        var numberOfSteps = 100;

        for (int i = 0; i < numberOfClients; i++)
        {
            var client = new KafkaLatencyClient(i, numberOfSteps);
            tasks.Add(Task.Run(() => client.Test()));
        }

        await Task.WhenAll(tasks);
    }
}