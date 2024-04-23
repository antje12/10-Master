using System.Diagnostics;

namespace RestConsoleApp;

class Program
{
    static async Task Main()
    {
        string uri = "http://localhost:8080/RestService/Version";

        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback =
            (sender, cert, chain, sslPolicyErrors) => { return true; };
        HttpClient client = new HttpClient(clientHandler);

        string path = @"D:\Git\10-Master\Experiments\Experiment1_Results\Rest.txt";
        File.Delete(path);
        
        var stopwatch = new Stopwatch();
        using (StreamWriter sw = File.AppendText(path))
        {
            for (int i = 0; i < 1010; i++)
            {
                await Test(client, uri, stopwatch, sw);
            }
        }
    }

    private static async Task Test(HttpClient client, string uri, Stopwatch stopwatch, StreamWriter sw)
    {
        stopwatch.Restart();
        using var httpResponse = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299
        try
        {
            var result = httpResponse.Content.ReadAsStringAsync().Result;
            stopwatch.Stop();
            var elapsed_time = stopwatch.ElapsedMilliseconds;
            //Console.WriteLine($"HTTP result in {elapsed_time} ms");
            sw.WriteLine(elapsed_time);
        }
        catch
        {
            Console.WriteLine("HTTP Response was invalid or could not be deserialized.");
            throw;
        }
    }
}