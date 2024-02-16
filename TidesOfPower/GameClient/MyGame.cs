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
    }

    protected override async void Initialize()
    {
        // TODO: Add your initialization logic here
        screenWidth = GraphicsDevice.Viewport.Width;
        screenHeight = GraphicsDevice.Viewport.Height;
        _camera = new Camera();

        base.Initialize();

        _cts = new CancellationTokenSource();
        IConsumer<Output>.ProcessMessage action = ProcessMessage;
        await Task.Run(() => _consumer.Consume(Output, action, _cts.Token), _cts.Token);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        avatarTexture = Content.Load<Texture2D>("square");
        islandTexture = Content.Load<Texture2D>("island");
        oceanTexture = Content.Load<Texture2D>("ocean");

        var playerPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        var enemyPosition = new Vector2(0, 0);
        player = new Entities.Player(playerPosition, avatarTexture, _camera, playerId, _producer);
        enemy = new Entities.Enemy(enemyPosition, avatarTexture);
    }

    private void ProcessMessage(string key, Output value)
    {
        player.Position = new Vector2(value.Location.X, value.Location.Y);
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

        DrawBackground(gameTime, oceanTexture);

        //Rectangle island = new Rectangle(screenWidth / 2, screenHeight / 2, 64, 64);
        //_spriteBatch.Draw(islandTexture, island, Color.White);
        _spriteBatch.Draw(islandTexture, new Vector2(0, 0), Color.White);

        enemy.Draw(gameTime, _spriteBatch);
        player.Draw(gameTime, _spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawBackground(GameTime gameTime, Texture2D texture)
    {
        var startX = player.Position.X - screenWidth / 2 - texture.Width;
        var offsetX = startX % texture.Width;
        startX -= offsetX;

        var startY = player.Position.Y - screenHeight / 2 - texture.Height;
        var offsetY = startY % texture.Height;
        startY -= offsetY;

        var bgWidth = screenWidth + texture.Width * 2;
        var bgHeight = screenHeight + texture.Height * 2;

        Rectangle background = new Rectangle((int) startX, (int) startY, bgWidth, bgHeight);
        //_spriteBatch.Draw(oceanTexture, background, Color.White);
        // Draw the repeating texture using a loop to cover the entire destination rectangle
        for (int y = background.Top; y < background.Bottom; y += oceanTexture.Height)
        {
            for (int x = background.Left; x < background.Right; x += oceanTexture.Width)
            {
                _spriteBatch.Draw(oceanTexture, new Vector2(x, y), Color.White);
            }
        }
    }
}