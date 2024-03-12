using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameClient;
using Microsoft.Extensions.Logging;

MyGame gameInstance = new MyGame();

// Create and build the host
using IHost host = CreateHostBuilder(args, gameInstance).Build();

// Start the MonoGame application in a non-blocking way
_ = Task.Run(() =>
{
    gameInstance.Run(); // Start MonoGame application
});

// Run the background service(s)
host.Run(); // This blocks the thread and runs the background service(s)

// Define the method to create and configure the host builder
static IHostBuilder CreateHostBuilder(string[] args, MyGame game) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders(); // Clears all default logging providers
            logging.AddConsole(); // Adds console logging
        })
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<MyGame>(game);
            services.AddHostedService<SyncService>(); // Your custom hosted service
        });