using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Classes.Messages;
using ClassLibrary.Kafka;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClassLibrary.Interfaces;
using ClassLibrary.MongoDB;
using GameClient.Core;
using GameClient.Entities;
using Enemy = GameClient.Entities.Enemy;
using Player = GameClient.Entities.Player;

namespace GameClient;

public class MyGame : Game
{
    const string GroupId = "output-group";

    static Guid playerId = Guid.NewGuid();

    //private string Output = $"{KafkaTopic.LocalState}_{playerId.ToString()}";
    string Output = KafkaTopic.LocalState.ToString();

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

    private readonly MongoDbBroker _mongoBroker; // ToDo: Delete this

    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        LocalState = new List<Sprite>();

        _config = new KafkaConfig(GroupId);
        _admin = new KafkaAdministrator(_config);
        _admin.CreateTopic(KafkaTopic.Input);
        _admin.CreateTopic(Output);
        _producer = new KafkaProducer<Input>(_config);
        _consumer = new KafkaConsumer<LocalState>(_config);

        _mongoBroker = new MongoDbBroker(); // ToDo: Delete this
    }

    protected override async void Initialize()
    {
        // TODO: Add your initialization logic here
        screenWidth = GraphicsDevice.Viewport.Width;
        screenHeight = GraphicsDevice.Viewport.Height;
        _camera = new Camera();

        base.Initialize();

        _cts = new CancellationTokenSource();
        IConsumer<LocalState>.ProcessMessage action = ProcessMessage;
        await Task.Run(() => _consumer.Consume(Output, action, _cts.Token), _cts.Token);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        avatarTexture = Content.Load<Texture2D>("square");
        islandTexture = Content.Load<Texture2D>("island");
        oceanTexture = Content.Load<Texture2D>("ocean");

        var islandPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        var island = new Entities.Island(islandPosition, islandTexture);

        var enemyPosition = new Vector2(0, 0);
        var enemy = new Entities.Enemy(Guid.NewGuid(), enemyPosition, avatarTexture);

        var playerPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        var player = new Entities.Player(playerId, playerPosition, avatarTexture, _camera, _producer);

        var oceanPosition = new Vector2(0, 0);
        var ocean = new Entities.Ocean(oceanPosition, oceanTexture, player);

        LocalState.Add(ocean);
        LocalState.Add(island);
        LocalState.Add(enemy);
        LocalState.Add(player);
    }

    private void ProcessMessage(string key, LocalState value)
    {
        var player = LocalState.First(x => x is Player);
        player.Position = new Vector2(value.Location.X, value.Location.Y);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var player = LocalState.First(x => x is Player);
        Console.WriteLine($"Player location; {player.Position.X}:{player.Position.Y}");
        var temp = _mongoBroker.ReadScreen(new Coordinates()
        {
            X = player.Position.X,
            Y = player.Position.Y
        }); // ToDo: Delete this

        var enemies = temp.Where(x => x.Id != playerId).ToList();
        Console.WriteLine($"Enemise from server: {enemies.Count}");
        var localEnemies = LocalState.Where(x => x is Enemy).Select(x => (Agent)x).ToList();
        Console.WriteLine($"Enemise from local: {localEnemies.Count}");
        foreach (var enemy in enemies)
        {
            if (localEnemies.All(x => x._agentId != enemy.Id))
            {
                var enemyPosition = new Vector2(enemy.Location.X, enemy.Location.Y);
                var newEnemy = new Entities.Enemy(enemy.Id, enemyPosition, avatarTexture);
                LocalState.Add(newEnemy);
            }
        }

        foreach (var enemy in localEnemies)
        {
            if (enemies.All(x => x.Id != enemy._agentId))
            {
                LocalState.RemoveAll(x => x is Enemy && ((Enemy)x)._agentId == enemy._agentId);
            }
        }
        
        // TODO: Add your update logic here
        foreach (var sprite in LocalState)
        {
            sprite.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        // TODO: Add your drawing code here
        _spriteBatch.Begin(transformMatrix: _camera.Transform);
        //_spriteBatch.Begin();

        foreach (var sprite in LocalState)
        {
            sprite.Draw(gameTime, _spriteBatch);
        }
        //enemy.Draw(gameTime, _spriteBatch);
        //player.Draw(gameTime, _spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}