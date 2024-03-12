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
    public Guid PlayerId = Guid.NewGuid();
    
    const string GroupId = "output-group";
    public static KafkaTopic OutputTopic = KafkaTopic.Input;

    readonly KafkaConfig _config;
    readonly ProtoKafkaProducer<Input> _producer;

    public Texture2D oceanTexture; //64x64
    public Texture2D islandTexture; //64x64
    public Texture2D avatarTexture; //50x50
    public Texture2D projectileTexture; //10x10

    Camera _camera;
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    public static int screenHeight; //480
    public static int screenWidth; //800

    public Player Player;
    public List<Sprite> LocalState = new List<Sprite>();
    public readonly object _lockObject = new object();
    public Dictionary<string, long> dict = new Dictionary<string, long>();
    
    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _config = new KafkaConfig(GroupId, true);
        _producer = new ProtoKafkaProducer<Input>(_config);
    }

    protected override async void Initialize()
    {
        screenWidth = GraphicsDevice.Viewport.Width;
        screenHeight = GraphicsDevice.Viewport.Height;
        _camera = new Camera();
        base.Initialize();

        var playerPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        Player = new Player(this, PlayerId, playerPosition, avatarTexture, _camera, _producer);

        var oceanPosition = new Vector2(0, 0);
        var ocean = new Ocean(oceanPosition, oceanTexture, Player);
        
        var islandPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        var island = new Island(islandPosition, islandTexture);

        LocalState.Add(ocean);
        LocalState.Add(island);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        avatarTexture = Content.Load<Texture2D>("circle");
        islandTexture = Content.Load<Texture2D>("island");
        oceanTexture = Content.Load<Texture2D>("ocean");
        projectileTexture = Content.Load<Texture2D>("small-circle");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        lock (_lockObject)
        {
            foreach (var sprite in LocalState)
            {
                sprite.Update(gameTime);
            }
        }
        Player.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(transformMatrix: _camera.Transform);
        lock (_lockObject)
        {
            foreach (var sprite in LocalState)
            {
                sprite.Draw(_spriteBatch);
            }
        }
        Player.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}