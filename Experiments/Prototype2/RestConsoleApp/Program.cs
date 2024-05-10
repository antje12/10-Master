using System.Diagnostics; 
using System.Text; 
using ClassLibrary; 
using Newtonsoft.Json; 
 
namespace RestConsoleApp; 
 
class Program 
{ 
    private static List<long> _results = new(); 
     
    static async Task Main() 
    { 
        Console.WriteLine("Rest test started!"); 
        string uri = "http://localhost:8080/RestService/Test"; 
         
        string path = @"D:\Git\10-Master\Experiments\Experiment1_Results\Rest.txt"; 
        File.Delete(path); 
         
        HttpClientHandler clientHandler = new HttpClientHandler(); 
        clientHandler.ServerCertificateCustomValidationCallback = 
            (sender, cert, chain, sslPolicyErrors) => { return true; }; 
        HttpClient client = new HttpClient(clientHandler); 
 
        var stopwatch = new Stopwatch(); 
        await Test(client, uri, stopwatch, null); 
        _results = new List<long>(); 
        File.Delete(path); 
         
        using (StreamWriter sw = File.AppendText(path))  
        {  
            for (int i = 0; i < 100; i++)  
            {  
                await Test(client, uri, stopwatch, sw);  
            }  
        } 
        Console.WriteLine("Rest test done!");                 
        Console.WriteLine($"results: {_results.Count}, avg: {_results.Average()}, min: {_results.Min()}, max: {_results.Max()}"); 
    } 
 
    private static async Task Test(HttpClient client, string uri, Stopwatch stopwatch, StreamWriter? sw)  
    { 
        stopwatch.Restart(); 
        var message = new MessageData(); 
        var json = JsonConvert.SerializeObject(message);     
        var content = new StringContent(json, Encoding.UTF8, "application/json"); 
 
        using var httpResponse = await client.PostAsync(uri, content); 
        httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299 
        try 
        { 
            var result = httpResponse.Content.ReadAsStringAsync().Result; 
            stopwatch.Stop(); 
            var elapsedTime = stopwatch.ElapsedMilliseconds; 
            //Console.WriteLine($"HTTP result in {elapsed_time} ms"); 
            _results.Add(elapsedTime); 
            sw?.WriteLine(elapsedTime);  
        } 
        catch 
        { 
            Console.WriteLine("HTTP Response was invalid or could not be deserialized."); 
            throw; 
        } 
    } 
}