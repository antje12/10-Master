using ClassLibrary.MongoDB;
using ClassLibrary.Redis;
using TestConsole.Tests;

Console.WriteLine("Hello, World!");

RedisBroker redisBroker = new RedisBroker();
redisBroker.Connect(true);
redisBroker.Clean();

MongoDbBroker mongoBroker = new MongoDbBroker();
mongoBroker.Connect(true);
mongoBroker.CleanDB();

//var uptime = new Uptime();
//await uptime.Test();

var latency = new Latency();
await latency.Test();
