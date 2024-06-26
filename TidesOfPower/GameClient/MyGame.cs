﻿using System;
using System.Collections.Generic;
using ClassLibrary.Domain;
using ClassLibrary.Kafka;
using ClassLibrary.Messages.Protobuf;
using ClassLibrary.Redis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameClient.Core;
using GameClient.Sprites;

namespace GameClient;

public class MyGame : Game
{
    private string _groupId = "output-group";
    public KafkaTopic OutputTopic = KafkaTopic.Input;
    private KafkaConfig _config;
    private KafkaProducer<Input_M> _producer;

    private SpriteFont _font;

    public Texture2D OceanTexture;
    public Texture2D IslandTexture;

    public Texture2D PlayerTexture;
    public Texture2D EnemyTexture;
    public Texture2D TreasureTexture;
    public Texture2D CoinTexture;

    public Texture2D ShipTexture;
    public Texture2D ShipTexture2;

    public Texture2D ProjectileTexture;

    private UI _ui;
    private Camera _camera;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public int ScreenHeight; //480
    public int ScreenWidth; //800

    public Player_S Player;
    public List<Sprite> LocalState = new();
    public readonly object LockObject = new();
    public Dictionary<string, DateTime> EventTimes = new();
    
    public int Latency = 0;
    
    internal RedisBroker RedisBroker;
    
    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _config = new KafkaConfig(_groupId, true);
        _producer = new KafkaProducer<Input_M>(_config);
        
        RedisBroker = new RedisBroker();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        IslandTexture = Content.Load<Texture2D>("environment/island_rough");
        OceanTexture = Content.Load<Texture2D>("environment/ocean");
        ProjectileTexture = Content.Load<Texture2D>("projectiles/cannon_ball");
        _font = Content.Load<SpriteFont>("fonts/Arial16");

        PlayerTexture = Content.Load<Texture2D>("avatars/pirate_1");
        EnemyTexture = Content.Load<Texture2D>("avatars/pirate_7");

        CoinTexture = Content.Load<Texture2D>("treasure/gold_coin");
        TreasureTexture = Content.Load<Texture2D>("treasure/gold_chest");
        ShipTexture = Content.Load<Texture2D>("ships/ship_1");
        ShipTexture2 = Content.Load<Texture2D>("ships/ship_2");
    }

    protected override void Initialize()
    {
        ScreenWidth = GraphicsDevice.Viewport.Width;
        ScreenHeight = GraphicsDevice.Viewport.Height;
        base.Initialize(); // Runs LoadContent

        _camera = new Camera(this);
        var playerLocation = new Coordinates(ScreenWidth / 2, ScreenHeight / 2);
        Player = new Player_S(this, PlayerTexture, ShipTexture, _camera, _producer, new Player("Player", 0, Guid.NewGuid(), playerLocation, 100, 100));
        _ui = new UI(_font, _camera, this);
        
        Player.Id = Guid.Parse("93e2fab7-0856-4560-b53b-e6aeca1656c9");
        
        LocalState.Add(new Ocean_S(this, OceanTexture, new Ocean()));
        LocalState.Add(GetIsland(64, 64));
        LocalState.Add(GetIsland(64, 448));
        LocalState.Add(GetIsland(448, 64));
        LocalState.Add(GetIsland(448, 448));
        
        RedisBroker.Connect(true);
    }
    
    private Island_S GetIsland(int x , int y)
    {
        int fromX = x;
        var toX = fromX + 320;
        var fromY = y;
        var toY = fromY + 320;
        return new Island_S(IslandTexture, new Island(fromX, toX, fromY, toY));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        lock (LockObject)
        {
            for (var i = LocalState.Count - 1; i >= 0; i--)
            {
                var sprite = LocalState[i];
                sprite.Update(gameTime);
                if (sprite is Enemy_S or Projectile_S or Treasure_S && IsOffScreen(sprite) || sprite is Projectile_S {TimeToLive: <= 0})
                {
                    LocalState.RemoveAt(i);
                }
            }
        }

        Player.Update(gameTime);
        base.Update(gameTime);
    }

    private bool IsOffScreen(Sprite sprite)
    {
        var startX = Player.Location.X - ScreenWidth / 2 + 1;
        var endX = Player.Location.X + ScreenWidth / 2 - 1;
        var startY = Player.Location.Y - ScreenHeight / 2 + 1;
        var endY = Player.Location.Y + ScreenHeight / 2 - 1;

        if (sprite.Location.X <= startX || endX <= sprite.Location.X ||
            sprite.Location.Y <= startY || endY <= sprite.Location.Y)
        {
            return true;
        }

        return false;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(transformMatrix: _camera.Transform);
        lock (LockObject)
        {
            foreach (var sprite in LocalState)
            {
                sprite.Draw(_spriteBatch);
            }
        }

        Player.Draw(_spriteBatch);
        _ui.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
    
    protected override void OnExiting(Object sender, EventArgs args)
    {
        base.OnExiting(sender, args);
        RedisBroker.DeleteEntity(Player.Id);
        Environment.Exit(1);
    }
}