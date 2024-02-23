﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Messages;
using ClassLibrary.Kafka;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClassLibrary.Interfaces;
using GameClient.Core;
using GameClient.Entities;

namespace GameClient;

public class MyGame : Game
{
    static Guid PlayerId = Guid.NewGuid();
    const string GroupId = "output-group";
    static string InputTopic = $"{KafkaTopic.LocalState}_{PlayerId}";
    public static KafkaTopic OutputTopic = KafkaTopic.Input;

    static CancellationTokenSource _cts;
    readonly KafkaConfig _config;
    readonly KafkaAdministrator _admin;
    readonly KafkaProducer<Input> _producer;
    readonly KafkaConsumer<LocalState> _consumer;

    Texture2D oceanTexture; //64x64
    Texture2D islandTexture; //64x64
    Texture2D avatarTexture; //50x50

    Camera _camera;
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    public static int screenHeight; //480
    public static int screenWidth; //800

    private List<Sprite> LocalState;

    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        LocalState = new List<Sprite>();

        _config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(_config);
        _producer = new KafkaProducer<Input>(_config);
        _consumer = new KafkaConsumer<LocalState>(_config);
    }

    protected override async void Initialize()
    {
        screenWidth = GraphicsDevice.Viewport.Width;
        screenHeight = GraphicsDevice.Viewport.Height;
        _camera = new Camera();
        base.Initialize();

        _cts = new CancellationTokenSource();
        await _admin.CreateTopic(InputTopic);
        IConsumer<LocalState>.ProcessMessage action = ProcessMessage;
        await Task.Run(() => _consumer.Consume(InputTopic, action, _cts.Token), _cts.Token);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        avatarTexture = Content.Load<Texture2D>("square");
        islandTexture = Content.Load<Texture2D>("island");
        oceanTexture = Content.Load<Texture2D>("ocean");

        //var enemyPosition = new Vector2(0, 0);
        //var enemy = new Enemy(Guid.NewGuid(), enemyPosition, avatarTexture);

        var playerPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        var player = new Player(PlayerId, playerPosition, avatarTexture, _camera, _producer);

        var oceanPosition = new Vector2(0, 0);
        var ocean = new Ocean(oceanPosition, oceanTexture, player);

        var islandPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        var island = new Island(islandPosition, islandTexture);

        LocalState.Add(ocean);
        LocalState.Add(island);
        //LocalState.Add(enemy);
        LocalState.Add(player);
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
        }
    }

    private void FullSync(LocalState value)
    {
        DeltaSync(value);
        
        var onlineAvatarIds = value.Avatars.Select(x => x.Id).ToList();
        var localAvatarIds = LocalState.Where(x => x is Agent).Select(x => ((Agent) x)._agentId).ToList();
        foreach (var localAvatarId in localAvatarIds)
        {
            if (!onlineAvatarIds.Contains(localAvatarId))
            {
                LocalState.RemoveAll(x => x is Agent y && y._agentId == localAvatarId);
            }
        }
    }

    private void DeltaSync(LocalState value)
    {
        foreach (var avatar in value.Avatars)
        {
            var localAvatar = LocalState.FirstOrDefault(x => x is Agent y && y._agentId == avatar.Id);
            if (localAvatar != null)
            {
                localAvatar.Position = new Vector2(avatar.Location.X, avatar.Location.Y);
            }
            else
            {
                var newAvatar = new Enemy(avatar.Id, new Vector2(avatar.Location.X, avatar.Location.Y), avatarTexture);
                LocalState.Add(newAvatar);
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        foreach (var sprite in LocalState)
        {
            sprite.Update(gameTime);
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(transformMatrix: _camera.Transform);
        foreach (var sprite in LocalState)
        {
            sprite.Draw(gameTime, _spriteBatch);
        }
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}