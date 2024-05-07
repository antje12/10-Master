namespace TestConsole.Tests;

public class Latency
{
    public async Task Test()
    {
        var _path = @"D:\Git\10-Master\Experiments\Scalability.txt";
        File.Delete(_path);

        var tasks = new List<Task>();
        var numberOfClients = 10;
        var index = 0;
        
        //while (true)
        //{
        //    AddBatch(index, numberOfClients, tasks);
        //    index += 10;
        //
        //    var timestamp = DateTime.UtcNow;
        //    using (StreamWriter file = File.AppendText(_path))
        //    {
        //        file.WriteLine($"{timestamp:o}");
        //    }
        //
        //    Console.WriteLine($"Added 10 - {tasks.Count} clients running");
        //    Thread.Sleep(30000);
        //}

        AddBatch(index, numberOfClients, tasks);
        await Task.WhenAll(tasks);
    }

    private static void AddBatch(int index, int numberOfClients, List<Task> tasks)
    {
        for (int i = index; i < index + numberOfClients; i++)
        {
            var client = new KafkaLatencyClient(i);
            tasks.Add(Task.Run(() => client.Test()));
        }
    }
}