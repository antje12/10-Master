using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using Microsoft.Extensions.Hosting;
using ClassLibrary.Interfaces;
using GameClient.Entities;
using Microsoft.Xna.Framework;
using Projectile = GameClient.Entities.Projectile;
using SyncType = ClassLibrary.Messages.Protobuf.SyncType;

namespace GameClient;

public class SyncService : BackgroundService
{
    const string GroupId = "output-group";
    private KafkaTopic InputTopic = KafkaTopic.LocalState;

    readonly KafkaConfig _config;
    private readonly KafkaAdministrator _admin;
    readonly ProtoKafkaConsumer<LocalState> _consumer;

    private MyGame game;
    
    public bool IsRunning { get; private set; }
    
    public SyncService(MyGame game)
    {
        Console.WriteLine("SyncService Created!");
        _config = new KafkaConfig(GroupId, true);
        _admin = new KafkaAdministrator(_config);
        _consumer = new ProtoKafkaConsumer<LocalState>(_config);
        this.game = game;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        //https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        IsRunning = true;
        Console.WriteLine($"SyncService started");

        await _admin.CreateTopic($"{InputTopic}_{game.PlayerId}");
        IProtoConsumer<LocalState>.ProcessMessage action = ProcessMessage;
        await _consumer.Consume($"{InputTopic}_{game.PlayerId}", action, ct);

        IsRunning = false;
        Console.WriteLine($"SyncService stopped");
    }

    private void ProcessMessage(string key, LocalState value)
    {
        switch (value.Sync)
        {
            case SyncType.Full:
                var startTime = game.dict[value.EventId];
                game.dict.Remove(value.EventId);
                var endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var timeDiff = endTime - startTime;
                Console.WriteLine($"Latency = {timeDiff} ms");
                FullSync(value);
                break;
            case SyncType.Delta:
                DeltaSync(value);
                break;
            case SyncType.Delete:
                DeleteSync(value);
                break;
        }
    }

    private void FullSync(LocalState value)
    {
        DeltaSync(value);

        var onlineAvatarIds = value.Avatars.Select(x => x.Id).ToList();
        var onlineProjectileIds = value.Projectiles.Select(x => x.Id).ToList();

        if (game.LocalState.OfType<Agent>().Any(x => !onlineAvatarIds.Contains(x._agentId.ToString())) ||
            game.LocalState.OfType<Projectile>().Any(x => !onlineProjectileIds.Contains(x._id.ToString())))
        {
            lock (game._lockObject)
            {
                game.LocalState.RemoveAll(x => x is Agent y && !onlineAvatarIds.Contains(y._agentId.ToString()));
                game.LocalState.RemoveAll(x => x is Projectile y && !onlineProjectileIds.Contains(y._id.ToString()));
                Console.WriteLine($"LocalState count {game.LocalState.Count}");
            }
        }
    }

    private void DeltaSync(LocalState value)
    {
        foreach (var avatar in value.Avatars)
        {
            if (avatar.Id == game.PlayerId.ToString())
            {
                game.Player.Position = new Vector2(avatar.Location.X, avatar.Location.Y);
                continue;
            }

            var localAvatar = game.LocalState.FirstOrDefault(x => x is Agent y && y._agentId.ToString() == avatar.Id);
            if (localAvatar == null)
            {
                lock (game._lockObject)
                {
                    game.LocalState.Add(
                        new Enemy(Guid.Parse(avatar.Id), new Vector2(avatar.Location.X, avatar.Location.Y),
                            game.avatarTexture));
                    Console.WriteLine($"LocalState count {game.LocalState.Count}");
                }
            }
            else
                localAvatar.Position = new Vector2(avatar.Location.X, avatar.Location.Y);
        }

        foreach (var projectile in value.Projectiles)
        {
            var localAvatar = game.LocalState.FirstOrDefault(x => x is Projectile y && y._id.ToString() == projectile.Id);
            if (localAvatar == null)
            {
                lock (game._lockObject)
                {
                    game.LocalState.Add(
                        new Projectile(Guid.Parse(projectile.Id),
                            new Vector2(projectile.Location.X, projectile.Location.Y),
                            game.projectileTexture));
                    Console.WriteLine($"LocalState count {game.LocalState.Count}");
                }
            }
            else
                localAvatar.Position = new Vector2(projectile.Location.X, projectile.Location.Y);
        }
    }

    private void DeleteSync(LocalState value)
    {
        var deleteAvatarIds = value.Avatars.Select(x => x.Id).ToList();
        var deleteProjectileIds = value.Projectiles.Select(x => x.Id).ToList();

        if (game.LocalState.OfType<Agent>().Any(x => deleteAvatarIds.Contains(x._agentId.ToString())) ||
            game.LocalState.OfType<Projectile>().Any(x => deleteProjectileIds.Contains(x._id.ToString())))
        {
            lock (game._lockObject)
            {
                game.LocalState.RemoveAll(x => x is Agent y && deleteAvatarIds.Contains(y._agentId.ToString()));
                game.LocalState.RemoveAll(x => x is Projectile y && deleteProjectileIds.Contains(y._id.ToString()));
                Console.WriteLine($"LocalState count {game.LocalState.Count}");
            }
        }

        if (deleteAvatarIds.Contains(game.PlayerId.ToString()))
        {
            Console.WriteLine("Died!");
        }
    }
}