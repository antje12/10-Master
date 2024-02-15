using System;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Classes.Client;
using ClassLibrary.Kafka;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClassLibrary.Interfaces;
using GameClient.Core;

namespace GameClient;

public class MyGame : Game
{
    private const string GroupId = "output-group";

    private static Guid playerId = Guid.NewGuid();

    //private string Output = $"{KafkaTopic.LocalState}_{playerId.ToString()}";
    private string Output = KafkaTopic.LocalState.ToString();

    private static CancellationTokenSource _cts;
    private readonly KafkaConfig _config;
    private readonly KafkaAdministrator _admin;
    private readonly KafkaProducer<Input> _producer;
    private readonly KafkaConsumer<Output> _consumer;

    Texture2D avatarTexture; //50x50
    Texture2D islandTexture; //64x64
    Texture2D oceanTexture; //64x64

    private Entities.Player player;
    private Entities.Enemy enemy;

    private Camera _camera;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static int screenHeight; //480
    public static int screenWidth; //800

    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(_config);
        _admin.CreateTopic(KafkaTopic.Input);
        _admin.CreateTopic(Output);
        _producer = new KafkaProducer<Input>(_config);
        _consumer = new KafkaConsumer<Output>(_config);

        _cts = new CancellationTokenSource();
        IConsumer<Output>.ProcessMessage action = ProcessMessage;
        Task.Run(() => _consumer.Consume(Output, action, _cts.Token), _cts.Token);
    }

    private void ProcessMessage(string key, Output value)
    {
        player.Position = new Vector2(value.Location.X, value.Location.Y);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        screenWidth = GraphicsDevice.Viewport.Width;
        screenHeight = GraphicsDevice.Viewport.Height;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        avatarTexture = Content.Load<Texture2D>("square");
        islandTexture = Content.Load<Texture2D>("island");
        oceanTexture = Content.Load<Texture2D>("ocean");

        _camera = new Camera();
        var playerPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        var enemyPosition = new Vector2(0, 0);
        player = new Entities.Player(playerPosition, avatarTexture, _camera, playerId, _producer);
        enemy = new Entities.Enemy(enemyPosition, avatarTexture);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        enemy.Update(gameTime);
        player.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);


        // TODO: Add your drawing code here
        _spriteBatch.Begin(transformMatrix: _camera.Transform);
        //_spriteBatch.Begin();

        var testX = player.Position.X - 64;
        //var offsetX = testX % 64;
        //testX -= offsetX;

        var testY = player.Position.Y - 64;
        //var offsetY = testY % 64;
        //testY -= offsetY;

        var bgStartX = 0;
        var bgStartY = 0;

        Console.WriteLine($"{testX}:{testY}");

        var bgWidth = screenWidth;
        var bgHeight = screenHeight;

        Rectangle background = new Rectangle((int) testX, (int) testY, 128, 128);
        //_spriteBatch.Draw(oceanTexture, background, Color.White);
        // Draw the repeating texture using a loop to cover the entire destination rectangle
        for (int y = background.Top; y < background.Bottom; y += oceanTexture.Height)
        {
            for (int x = background.Left; x < background.Right; x += oceanTexture.Width)
            {
                _spriteBatch.Draw(oceanTexture, new Vector2(x, y), Color.White);
            }
        }

        //Rectangle island = new Rectangle(screenWidth / 2, screenHeight / 2, 64, 64);
        //_spriteBatch.Draw(islandTexture, island, Color.White);
        
        _spriteBatch.Draw(islandTexture, new Vector2(0, 0), Color.White);

        enemy.Draw(gameTime, _spriteBatch);
        player.Draw(gameTime, _spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}