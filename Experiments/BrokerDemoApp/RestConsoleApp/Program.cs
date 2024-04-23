using System.Diagnostics;

string uri = "http://localhost:5051/RestService/Version";

HttpClientHandler clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback =
    (sender, cert, chain, sslPolicyErrors) => { return true; };
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
