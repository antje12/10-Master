using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Classes.Data;
using ClassLibrary.Kafka;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClassLibrary.Interfaces;
using GameClient.Core;
using GameClient.Entities;
using ClassLibrary.Messages.Protobuf;
using Projectile = GameClient.Entities.Projectile;
using SyncType = ClassLibrary.Messages.Protobuf.SyncType;

namespace GameClient;

public class MyGame : Game
{
    const string GroupId = "output-group";
    private KafkaTopic InputTopic = KafkaTopic.LocalState;
    public static KafkaTopic OutputTopic = KafkaTopic.Input;

    static CancellationTokenSource _cts;
    readonly KafkaConfig _config;
    readonly KafkaAdministrator _admin;
    readonly ProtoKafkaProducer<Input> _producer;
    readonly ProtoKafkaConsumer<LocalState> _consumer;

    Texture2D oceanTexture; //64x64
    Texture2D islandTexture; //64x64
    Texture2D avatarTexture; //50x50
    Texture2D projectileTexture; //10x10

    Camera _camera;
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    public static int screenHeight; //480
    public static int screenWidth; //800

    private Player Player;
    private List<Sprite> LocalState;

    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _config = new KafkaConfig(GroupId, true);
        _admin = new KafkaAdministrator(_config);
        _producer = new ProtoKafkaProducer<Input>(_config);
        _consumer = new ProtoKafkaConsumer<LocalState>(_config);
    }

    protected override async void Initialize()
    {
        screenWidth = GraphicsDevice.Viewport.Width;
        screenHeight = GraphicsDevice.Viewport.Height;
        _camera = new Camera();
        base.Initialize();

        LocalState = new List<Sprite>();
        
        var PlayerId = Guid.NewGuid();
        var playerPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        Player = new Player(PlayerId, playerPosition, avatarTexture, _camera, _producer);

        var oceanPosition = new Vector2(0, 0);
        var ocean = new Ocean(oceanPosition, oceanTexture, Player);

        var islandPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        var island = new Island(islandPosition, islandTexture);

        LocalState.Add(ocean);
        LocalState.Add(island);

        _cts = new CancellationTokenSource();
        await _admin.CreateTopic($"{InputTopic}_{Player._agentId}");
        IProtoConsumer<LocalState>.ProcessMessage action = ProcessMessage;
        await Task.Run(() => _consumer.Consume($"{InputTopic}_{Player._agentId}", action, _cts.Token), _cts.Token);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        avatarTexture = Content.Load<Texture2D>("circle");
        islandTexture = Content.Load<Texture2D>("island");
        oceanTexture = Content.Load<Texture2D>("ocean");
        projectileTexture = Content.Load<Texture2D>("small-circle");
    }

    private void ProcessMessage(string key, LocalState value)
    {
        switch (value.Sync)
        {
            case SyncType.Full:
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
        LocalState.RemoveAll(x => x is Agent y && !onlineAvatarIds.Contains(y._agentId.ToString()));

        var onlineProjectileIds = value.Projectiles.Select(x => x.Id).ToList();
        LocalState.RemoveAll(x => x is Projectile y && !onlineProjectileIds.Contains(y._id.ToString()));
    }

    private void DeltaSync(LocalState value)
    {
        foreach (var avatar in value.Avatars)
        {
            if (avatar.Id == Player._agentId.ToString())
            {
                Player.Position = new Vector2(avatar.Location.X, avatar.Location.Y);
                continue;
            }
            
            var localAvatar = LocalState.FirstOrDefault(x => x is Agent y && y._agentId.ToString() == avatar.Id);
            if (localAvatar == null)
                LocalState.Add(
                    new Enemy(Guid.Parse(avatar.Id), new Vector2(avatar.Location.X, avatar.Location.Y), avatarTexture));
            else
                localAvatar.Position = new Vector2(avatar.Location.X, avatar.Location.Y);
        }

        foreach (var projectile in value.Projectiles)
        {
            var localAvatar = LocalState.FirstOrDefault(x => x is Projectile y && y._id.ToString() == projectile.Id);
            if (localAvatar == null)
                LocalState.Add(
                    new Projectile(Guid.Parse(projectile.Id), new Vector2(projectile.Location.X, projectile.Location.Y),
                        projectileTexture));
            else
                localAvatar.Position = new Vector2(projectile.Location.X, projectile.Location.Y);
        }
    }

    private void DeleteSync(LocalState value)
    {
        var deleteAvatarIds = value.Avatars.Select(x => x.Id).ToList();
        LocalState.RemoveAll(x => x is Agent y && deleteAvatarIds.Contains(y._agentId.ToString()));

        if (deleteAvatarIds.Contains(Player._agentId.ToString()))
        {
            Console.WriteLine("Died!");
        }
        
        var deleteProjectileIds = value.Projectiles.Select(x => x.Id).ToList();
        LocalState.RemoveAll(x => x is Projectile y && deleteProjectileIds.Contains(y._id.ToString()));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        for (var i = 0; i < LocalState.Count; i++)
        {
            var sprite = LocalState[i];
            sprite.Update(gameTime);
        }
        Player.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(transformMatrix: _camera.Transform);
        for (var i = 0; i < LocalState.Count; i++)
        {
            var sprite = LocalState[i];
            sprite.Draw(gameTime, _spriteBatch);
        }
        Player.Draw(gameTime, _spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}