using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameClient;
using GameClient.Services;
using Microsoft.Extensions.Logging;

MyGame gameInstance = new MyGame();

using IHost host = CreateHostBuilder(args, gameInstance).Build();

_ = Task.Run(() =>
{
    gameInstance.Run();
});
while (gameInstance.Player == null)
{
    Console.WriteLine("Zzz...");
    Thread.Sleep(1000);
}

host.Run();

static IHostBuilder CreateHostBuilder(string[] args, MyGame game) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders(); // Clear all default logging providers
            logging.AddConsole(); // Adds console logging
        })
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<MyGame>(game);
            services.AddHostedService<SyncService>();
        });