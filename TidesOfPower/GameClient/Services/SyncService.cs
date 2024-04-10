using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using Microsoft.Extensions.Hosting;
using ClassLibrary.Interfaces;
using GameClient.Core;
using GameClient.Sprites;
using Projectile = ClassLibrary.Classes.Domain.Projectile;
using Coordinates = ClassLibrary.Classes.Domain.Coordinates;

namespace GameClient.Services;

public class SyncService : BackgroundService
{
    private string _groupId = "output-group";
    private KafkaTopic _inputTopic = KafkaTopic.LocalState;
    private KafkaConfig _config;
    private KafkaAdministrator _admin;
    private ProtoKafkaConsumer<LocalState> _consumer;

    private MyGame _game;
    private LatencyList _latency = new(100);

    public SyncService(MyGame game)
    {
        Console.WriteLine("SyncService Created!");
        _config = new KafkaConfig(_groupId, true);
        _admin = new KafkaAdministrator(_config);
        _consumer = new ProtoKafkaConsumer<LocalState>(_config);
        _game = game;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();
        Console.WriteLine($"SyncService started");
        await _admin.CreateTopic($"{_inputTopic}_{_game.Player.Id}");
        IProtoConsumer<LocalState>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume($"{_inputTopic}_{_game.Player.Id}", action, ct);
        Console.WriteLine($"SyncService stopped");
    }

    private void ProcessMessage(string key, LocalState value)
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

    private void GetLatency(LocalState value)
    {
        var startTime = _game.EventTimes[value.EventId];
        _game.EventTimes.Remove(value.EventId);
        var endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var timeDiff = endTime - startTime;
        string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        _latency.Add(timeDiff);
        _game.Latency = _latency.GetAverage();
        Console.WriteLine($"Got {value.EventId} Latency = {timeDiff} ms - stamp: {timestampWithMs}");
    }

    private void FullSync(LocalState value)
    {
        var player = value.Agents.FirstOrDefault(x => x.Id == _game.Player.Id.ToString());
        if (player != null)
        {
            var xDiff = Math.Abs(_game.Player.Location.X - player.Location.X);
            var yDiff = Math.Abs(_game.Player.Location.Y - player.Location.Y);
            if (xDiff > 50 || yDiff > 50)
                _game.Player.Location = new Coordinates(player.Location.X, player.Location.Y);
            value.Agents.Remove(player);
        }
        
        DeltaSync(value);

        var onlineAvatarIds = value.Agents.Select(x => x.Id).ToList();
        //var onlineProjectileIds = value.Projectiles.Select(x => x.Id).ToList();

        if (_game.LocalState.OfType<Enemy_S>().Any(x => !onlineAvatarIds.Contains(x.Id.ToString()))) //||
            //game.LocalState.OfType<Projectile>().Any(x => !onlineProjectileIds.Contains(x._id.ToString())))
        {
            lock (_game.LockObject)
            {
                _game.LocalState.RemoveAll(x => x is Enemy_S y && !onlineAvatarIds.Contains(y.Id.ToString()));
                //game.LocalState.RemoveAll(x => x is Projectile y && !onlineProjectileIds.Contains(y._id.ToString()));
                string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
                Console.WriteLine($"LocalState count {_game.LocalState.Count} at {timestampWithMs}");
            }
        }
    }

    private void DeltaSync(LocalState value)
    {
        foreach (var avatar in value.Agents)
        {
            if (avatar.Id == _game.Player.Id.ToString())
            {
                throw new Exception("Major error!!!");
            }
            
            var localAvatar = _game.LocalState.FirstOrDefault(x => x is Enemy_S y && y.Id.ToString() == avatar.Id);
            if (localAvatar == null)
            {
                lock (_game.LockObject)
                {
                    _game.LocalState.Add(
                        new Enemy_S(_game.EnemyTexture, new Enemy(Guid.Parse(avatar.Id), new Coordinates(avatar.Location.X, avatar.Location.Y), 100, 100)));
                    string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
                    Console.WriteLine($"LocalState count {_game.LocalState.Count} at {timestampWithMs}");
                }
            }
            else
            {
                if (localAvatar is Enemy_S la)
                {
                    la.SetLocation(new Coordinates(avatar.Location.X, avatar.Location.Y));
                }
                else
                {
                    localAvatar.Location = new Coordinates(avatar.Location.X, avatar.Location.Y);
                }
            }
        }

        foreach (var projectile in value.Projectiles)
        {
            var localAvatar =
                _game.LocalState.FirstOrDefault(x => x is Projectile_S y && y.Id.ToString() == projectile.Id);
            if (localAvatar == null)
            {
                lock (_game.LockObject)
                {
                    _game.LocalState.Add(
                        new Projectile_S(_game.ProjectileTexture,
                            new Projectile(new Coordinates(projectile.Direction.X, projectile.Direction.Y), 100, 100,  Guid.Parse(projectile.Id), new Coordinates(projectile.Location.X, projectile.Location.Y))));
                    string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
                    Console.WriteLine($"LocalState count {_game.LocalState.Count} at {timestampWithMs}");
                }
            }
            else
            {
                localAvatar.Location = new Coordinates(projectile.Location.X,projectile.Location.Y);
            }
        }
    }

    private void DeleteSync(LocalState value)
    {
        var deleteAvatarIds = value.Agents.Select(x => x.Id).ToList();
        var deleteProjectileIds = value.Projectiles.Select(x => x.Id).ToList();

        if (_game.LocalState.OfType<Enemy_S>().Any(x => deleteAvatarIds.Contains(x.Id.ToString())) ||
            _game.LocalState.OfType<Projectile_S>().Any(x => deleteProjectileIds.Contains(x.Id.ToString())))
        {
            lock (_game.LockObject)
            {
                _game.LocalState.RemoveAll(x => x is Enemy_S y && deleteAvatarIds.Contains(y.Id.ToString()));
                _game.LocalState.RemoveAll(x => x is Projectile_S y && deleteProjectileIds.Contains(y.Id.ToString()));
                string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
                Console.WriteLine($"LocalState count {_game.LocalState.Count} at {timestampWithMs}");
            }
        }

        if (deleteAvatarIds.Contains(_game.Player.Id.ToString()))
        {
            Console.WriteLine("Player Died!");
        }
    }
}