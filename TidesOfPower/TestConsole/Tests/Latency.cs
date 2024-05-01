namespace TestConsole.Tests;

public class Latency
{
    public async Task Test()
    {
        var _path = @"D:\Git\10-Master\Experiments\Scalability.txt";
        File.Delete(_path);

        var tasks = new List<Task>();
        var numberOfClients = 10;
        var numberOfSteps = 100;
        var index = 0;
        while (true)
        {
            AddBatch(index, numberOfClients, numberOfSteps, tasks);
            index += 10;

            var timestamp = DateTime.UtcNow;
            using (StreamWriter file = File.AppendText(_path))
            {
                file.WriteLine($"{timestamp:o} - Added 10 - {tasks.Count} clients running");
            }

            Console.WriteLine($"Added 10 - {tasks.Count} clients running");
            Thread.Sleep(30000);
        }

        //await Task.WhenAll(tasks);
    }

    private static void AddBatch(int index, int numberOfClients, int numberOfSteps, List<Task> tasks)
    {
        for (int i = index; i < index + numberOfClients; i++)
        {
            var client = new KafkaLatencyClient(i, numberOfSteps);
            tasks.Add(Task.Run(() => client.Test()));
        }
    }
}