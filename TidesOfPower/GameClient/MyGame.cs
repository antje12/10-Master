using System;
using System.Collections.Generic;
using ClassLibrary.Kafka;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameClient.Core;
using GameClient.Entities;
using ClassLibrary.Messages.Protobuf;

namespace GameClient;

public class MyGame : Game
{
    private string _groupId = "output-group";
    public KafkaTopic OutputTopic = KafkaTopic.Input;
    private KafkaConfig _config;
    private ProtoKafkaProducer<Input> _producer;

    private Texture2D _oceanTexture; //64x64
    private Texture2D _islandTexture; //64x64
    public Texture2D AvatarTexture; //50x50
    public Texture2D ProjectileTexture; //10x10
    private SpriteFont _font; //10x10

    private UI _ui;
    private Camera _camera;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public int ScreenHeight; //480
    public int ScreenWidth; //800

    public Player Player;
    public List<Sprite> LocalState = new();
    public readonly object LockObject = new();
    public Dictionary<string, long> EventTimes = new();

    public Texture2D PlayerTexture;
    public Texture2D EnemyTexture;
    public Texture2D CoinTexture;
    public Texture2D TreasureTexture;
    private Coin _coin;
    private Treasure _treasure;
    private Hero _hero;
    
    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _config = new KafkaConfig(_groupId, true);
        _producer = new ProtoKafkaProducer<Input>(_config);
    }

    protected override void Initialize()
    {
        ScreenWidth = GraphicsDevice.Viewport.Width;
        ScreenHeight = GraphicsDevice.Viewport.Height;
        base.Initialize(); // Runs LoadContent

        _coin = new Coin(new Vector2(300, 300), CoinTexture);
        _treasure = new Treasure(new Vector2(100, 100), TreasureTexture);
        
        _camera = new Camera(this);
        var playerPosition = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
        Player = new Player(this, Guid.NewGuid(), playerPosition, AvatarTexture, _camera, _producer);
        _ui = new UI(_font, Player, this);

        _hero = new Hero(Player.Position, PlayerTexture);
        
        var oceanPosition = new Vector2(0, 0);
        var ocean = new Ocean(oceanPosition, _oceanTexture, Player, this);
        var islandPosition = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
        var island = new Island(islandPosition, _islandTexture);

        LocalState.Add(ocean);
        LocalState.Add(island);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        AvatarTexture = Content.Load<Texture2D>("circle");
        _islandTexture = Content.Load<Texture2D>("island");
        _oceanTexture = Content.Load<Texture2D>("ocean");
        ProjectileTexture = Content.Load<Texture2D>("small-circle");
        _font = Content.Load<SpriteFont>("Arial16");
        
        PlayerTexture = Content.Load<Texture2D>("player");
        EnemyTexture = Content.Load<Texture2D>("enemy");
        
        CoinTexture = Content.Load<Texture2D>("gold_coin");
        TreasureTexture = Content.Load<Texture2D>("gold_chest");
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
                if (sprite is Entities.Projectile && IsOffScreen(sprite))
                {
                    LocalState.RemoveAt(i);
                }
            }
        }
        Player.Update(gameTime);
        _coin.Update(gameTime);
        _treasure.Update(gameTime);
        _hero.Update(gameTime);
        base.Update(gameTime);
    }

    private bool IsOffScreen(Sprite sprite)
    {
        var startX = Player.Position.X - ScreenWidth / 2;
        var endX = Player.Position.X + ScreenWidth / 2;
        var startY = Player.Position.Y - ScreenHeight / 2;
        var endY = Player.Position.Y + ScreenHeight / 2;

        if (sprite.Position.X <= startX || endX <= sprite.Position.X ||
            sprite.Position.Y <= startY || endY <= sprite.Position.Y)
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
        _coin.Draw(_spriteBatch);
        _ui.Draw(_spriteBatch);
        _treasure.Draw(_spriteBatch);
        _hero.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}