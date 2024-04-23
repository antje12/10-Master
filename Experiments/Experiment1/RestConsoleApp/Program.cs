using System.Diagnostics;

namespace RestConsoleApp;

class Program
{
    private static List<long> _results;
    
    static async Task Main()
    {
        _results = new List<long>();
        Console.WriteLine("Rest test started!");
        string uri = "http://localhost:8080/RestService/Test";

        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback =
            (sender, cert, chain, sslPolicyErrors) => { return true; };
        HttpClient client = new HttpClient(clientHandler);

        string path = @"D:\Git\10-Master\Experiments\Experiment1_Results\Rest.txt";
        File.Delete(path);
        
        var stopwatch = new Stopwatch();
        using (StreamWriter sw = File.AppendText(path))
        {
            for (int i = 0; i <= 1000; i++)
            {
                Guid newGuid = Guid.NewGuid();
                string requestUri = $"{uri}/{newGuid}";
                await Test(client, requestUri, stopwatch, sw, i);
            }
        }
        Console.WriteLine("Rest test done!");                
        Console.WriteLine($"avg: {_results.Average()}, min: {_results.Min()}, max: {_results.Max()}");
    }

    private static async Task Test(HttpClient client, string uri, Stopwatch stopwatch, StreamWriter sw, int index)
    {
        stopwatch.Restart();
        using var httpResponse = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299
        try
        {
            var result = httpResponse.Content.ReadAsStringAsync().Result;
            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            //Console.WriteLine($"HTTP result in {elapsed_time} ms");
            if (index > 0)
            {
                _results.Add(elapsedTime);
                sw.WriteLine(elapsedTime);
            }
        }
        catch
        {
            Console.WriteLine("HTTP Response was invalid or could not be deserialized.");
            throw;
        }
    }
}