﻿using ClassLibrary.Classes;
using ClassLibrary.Kafka;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameClient;

public class MyGame : Game
{
    private const string Topic = "input";
    private const string GroupId = "msg-group";
    private const string KafkaServers = "localhost:19092";
    private const string SchemaRegistry = "localhost:8081";

    private readonly SchemaRegistryConfig _schemaRegistryConfig = new()
    {
        Url = SchemaRegistry
    };

    private readonly AdminClientConfig _adminConfig = new()
    {
        BootstrapServers = KafkaServers
    };

    private readonly ProducerConfig _producerConfig = new()
    {
        BootstrapServers = KafkaServers,
        Acks = Acks.None,
        LingerMs = 0,
        BatchSize = 1
    };

    private readonly ConsumerConfig _consumerConfig = new()
    {
        BootstrapServers = KafkaServers,
        GroupId = GroupId,
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    private readonly KafkaAdministrator _admin;
    private readonly KafkaProducer _producer;
    private readonly KafkaConsumer _consumer;
    
    Texture2D playerTexture;
    Vector2 playerPosition;
    Vector2 enemyPosition;
    Texture2D islandTexture;
    Texture2D oceanTexture;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _admin = new KafkaAdministrator(_adminConfig);
        _producer = new KafkaProducer(_producerConfig, _schemaRegistryConfig);
        _consumer = new KafkaConsumer(_consumerConfig, _schemaRegistryConfig);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        playerTexture = Content.Load<Texture2D>("square");
        islandTexture = Content.Load<Texture2D>("island");
        oceanTexture = Content.Load<Texture2D>("ocean");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        // TODO: Add your update logic here
        var kstate = Keyboard.GetState();

        Direction? dir = null;

        if (kstate.IsKeyDown(Keys.W))
        {
            playerPosition.Y -= 100 * (float) gameTime.ElapsedGameTime.TotalSeconds;
            dir = Direction.North;
        }

        if (kstate.IsKeyDown(Keys.S))
        {
            playerPosition.Y += 100 * (float) gameTime.ElapsedGameTime.TotalSeconds;
            dir = Direction.South;
        }

        if (kstate.IsKeyDown(Keys.A))
        {
            playerPosition.X -= 100 * (float) gameTime.ElapsedGameTime.TotalSeconds;
            dir = Direction.East;
        }

        if (kstate.IsKeyDown(Keys.D))
        {
            playerPosition.X += 100 * (float) gameTime.ElapsedGameTime.TotalSeconds;
            dir = Direction.West;
        }

        if (dir != null)
        {
            _producer.Produce(Topic, "me", dir.ToString() ?? string.Empty);
        }
        
        CorrectPosition(ref playerPosition);
        CorrectPosition(ref enemyPosition);

        base.Update(gameTime);
    }

    private void CorrectPosition(ref Vector2 position)
    {
        if (position.X > _graphics.PreferredBackBufferWidth - playerTexture.Width / 2)
            position.X = _graphics.PreferredBackBufferWidth - playerTexture.Width / 2;
        else if (position.X < playerTexture.Width / 2)
            position.X = playerTexture.Width / 2;

        if (position.Y > _graphics.PreferredBackBufferHeight - playerTexture.Height / 2)
            position.Y = _graphics.PreferredBackBufferHeight - playerTexture.Height / 2;
        else if (position.Y < playerTexture.Height / 2)
            position.Y = playerTexture.Height / 2;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        int screenWidth = GraphicsDevice.Viewport.Width;
        int screenHeight = GraphicsDevice.Viewport.Height;
        Rectangle background = new Rectangle(0, 0, screenWidth, screenHeight);
        Rectangle island = new Rectangle(screenWidth/2, screenHeight/2, 100, 100);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        _spriteBatch.Draw(oceanTexture, background, Color.White);
        _spriteBatch.Draw(islandTexture, island, Color.White);

        _spriteBatch.Draw(playerTexture, playerPosition, null, Color.Green, 0f,
            new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), Vector2.One,
            SpriteEffects.None,
            0f);
        _spriteBatch.Draw(playerTexture, enemyPosition, null, Color.Red, 0f,
            new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), Vector2.One,
            SpriteEffects.None,
            0f);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}