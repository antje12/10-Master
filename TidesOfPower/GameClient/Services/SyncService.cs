using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Domain;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using Microsoft.Extensions.Hosting;
using ClassLibrary.Interfaces;
using GameClient.Core;
using GameClient.Sprites;

namespace GameClient.Services;

public class SyncService : BackgroundService
{
    private string _groupId = "output-group";
    private KafkaTopic _inputTopic = KafkaTopic.LocalState;
    private KafkaConfig _config;
    private KafkaAdministrator _admin;
    private KafkaConsumer<LocalState_M> _consumer;

    private MyGame _game;
    private LatencyList _latency = new(100);

    public SyncService(MyGame game)
    {
        Console.WriteLine("SyncService Created!");
        _config = new KafkaConfig(_groupId, true);
        _admin = new KafkaAdministrator(_config);
        _consumer = new KafkaConsumer<LocalState_M>(_config);
        _game = game;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();
        Console.WriteLine($"SyncService started");
        await _admin.CreateTopic($"{_inputTopic}_{_game.Player.Id}");
        IProtoConsumer<LocalState_M>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume($"{_inputTopic}_{_game.Player.Id}", action, ct);
        Console.WriteLine($"SyncService stopped");
    }

    private void ProcessMessage(string key, LocalState_M value)
    {
        switch (value.Sync)
        {
            case Sync.Full:
                GetLatency(value);
                FullSync(value);
                break;
            case Sync.Delta:
                DeltaSync(value);
                break;
            case Sync.Delete:
                DeleteSync(value);
                break;
        }
    }

    private void GetLatency(LocalState_M value)
    {
        var endTime = DateTime.UtcNow;
        var startTime = _game.EventTimes[value.EventId];
        _game.EventTimes.Remove(value.EventId);
        var timeDiff = endTime - startTime;
        string timestampWithMs = endTime.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        if (_latency.GetAverage() == 0 && timeDiff.TotalMilliseconds > 500)
            return;
        _latency.Add(timeDiff.TotalMilliseconds);
        _game.Latency = _latency.GetAverage();
        Console.WriteLine($"Got {value.EventId} Latency = {timeDiff} ms - stamp: {timestampWithMs}");
    }

    private void FullSync(LocalState_M value)
    {
        var player = value.Agents.FirstOrDefault(x => x.Id == _game.Player.Id.ToString());
        if (player != null)
        {
            _game.Player.Score = player.Score;
            var xDiff = Math.Abs(_game.Player.Location.X - player.Location.X);
            var yDiff = Math.Abs(_game.Player.Location.Y - player.Location.Y);
            if (xDiff > 50 || yDiff > 50)
                _game.Player.Location = new Coordinates(player.Location.X, player.Location.Y);
            value.Agents.Remove(player);
        }
        
        DeltaSync(value);
    }

    private void DeltaSync(LocalState_M value)
    {
        foreach (var agent in value.Agents)
        {
            if (agent.Id == _game.Player.Id.ToString())
            {
                throw new Exception("Major error!!!");
            }
            
            var localAgent = _game.LocalState.FirstOrDefault(x => x is Enemy_S y && y.Id.ToString() == agent.Id);
            if (localAgent == null)
            {
                lock (_game.LockObject)
                {
                    _game.LocalState.Add(
                        new Enemy_S(_game.EnemyTexture, new Enemy(Guid.Parse(agent.Id), new Coordinates(agent.Location.X, agent.Location.Y), 100, 100)));
                    string timestampWithMs = DateTime.UtcNow.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
                    Console.WriteLine($"LocalState count {_game.LocalState.Count} at {timestampWithMs}");
                }
            }
            else
            {
                if (localAgent is Enemy_S la)
                {
                    la.SetLocation(new Coordinates(agent.Location.X, agent.Location.Y));
                }
                else
                {
                    localAgent.Location = new Coordinates(agent.Location.X, agent.Location.Y);
                }
            }
        }

        foreach (var projectile in value.Projectiles)
        {
            var localAgent =
                _game.LocalState.FirstOrDefault(x => x is Projectile_S y && y.Id.ToString() == projectile.Id);
            if (localAgent == null)
            {
                lock (_game.LockObject)
                {
                    _game.LocalState.Add(
                        new Projectile_S(_game.ProjectileTexture,
                            new Projectile(new Coordinates(projectile.Direction.X, projectile.Direction.Y), 100, 100, 100,  Guid.Parse(projectile.Id), new Coordinates(projectile.Location.X, projectile.Location.Y))));
                    string timestampWithMs = DateTime.UtcNow.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
                    Console.WriteLine($"LocalState count {_game.LocalState.Count} at {timestampWithMs}");
                }
            }
            else
            {
                localAgent.Location = new Coordinates(projectile.Location.X,projectile.Location.Y);
            }
        }

        foreach (var treasure in value.Treasures)
        {
            var localTreasure =
                _game.LocalState.FirstOrDefault(x => x is Treasure_S y && y.Id.ToString() == treasure.Id);
            if (localTreasure != null) continue;
            
            lock (_game.LockObject)
            {
                if (treasure.Value > 100)
                {
                    _game.LocalState.Add(new Treasure_S(_game.TreasureTexture, 4, 
                        new Treasure(treasure.Value, Guid.Parse(treasure.Id), new Coordinates(treasure.Location.X, treasure.Location.Y))));
                    continue;
                }
                _game.LocalState.Add(new Treasure_S(_game.CoinTexture, 6, 
                    new Treasure(treasure.Value, Guid.Parse(treasure.Id), new Coordinates(treasure.Location.X, treasure.Location.Y))));}
        }
    }

    private void DeleteSync(LocalState_M value)
    {
        var deleteAgentIds = value.Agents.Select(x => x.Id).ToList();
        var deleteProjectileIds = value.Projectiles.Select(x => x.Id).ToList();
        var deleteTreasureIds = value.Treasures.Select(x => x.Id).ToList();

        if (_game.LocalState.OfType<Enemy_S>().Any(x => deleteAgentIds.Contains(x.Id.ToString())) ||
            _game.LocalState.OfType<Projectile_S>().Any(x => deleteProjectileIds.Contains(x.Id.ToString())) ||
            _game.LocalState.OfType<Treasure_S>().Any(x => deleteTreasureIds.Contains(x.Id.ToString())))
        {
            lock (_game.LockObject)
            {
                _game.LocalState.RemoveAll(x => x is Enemy_S y && deleteAgentIds.Contains(y.Id.ToString()));
                _game.LocalState.RemoveAll(x => x is Projectile_S y && deleteProjectileIds.Contains(y.Id.ToString()));
                _game.LocalState.RemoveAll(x => x is Treasure_S y && deleteTreasureIds.Contains(y.Id.ToString()));
                string timestampWithMs = DateTime.UtcNow.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
                Console.WriteLine($"LocalState count {_game.LocalState.Count} at {timestampWithMs}");
            }
        }

        if (deleteAgentIds.Contains(_game.Player.Id.ToString()))
        {
            Console.WriteLine("Player Died!");
            lock (_game.LockObject)
            {
                _game.LocalState.RemoveAll(x => x is Enemy_S or Projectile_S or Treasure_S);
                string timestampWithMs = DateTime.UtcNow.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
                Console.WriteLine($"LocalState count {_game.LocalState.Count} at {timestampWithMs}");
            }
        }
    }
}