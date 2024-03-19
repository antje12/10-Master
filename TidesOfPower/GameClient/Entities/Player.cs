using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Kafka;
using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClassLibrary.Messages.Protobuf;

namespace GameClient.Entities;

public class Player : Agent
{
    private MyGame _game;
    private Camera _camera;
    private Vector2 _mousePosition;
    private ProtoKafkaProducer<Input> _producer;
    private AnimationManager _anims = new();

    private Coordinates _lastLocation;
    private List<GameKey> _lastKeyInput;
    private bool attacking;

    public int Latency = 0;
    public int Health = 100;
    public int Score = 0;

    public Player(MyGame game, Guid agentId, Vector2 position, Texture2D texture, Camera camera,
        ProtoKafkaProducer<Input> producer)
        : base(agentId, position, texture)
    {
        _game = game;
        _camera = camera;
        _producer = producer;
        _lastLocation = new Coordinates();
        _lastKeyInput = new List<GameKey>();
        
        _anims.AddAnimation(GameKey.Up, new(texture, 3, 4, 0.2f, 1));
        _anims.AddAnimation(GameKey.Right, new(texture, 3, 4, 0.2f, 2));
        _anims.AddAnimation(GameKey.Down, new(texture, 3, 4, 0.2f, 3));
        _anims.AddAnimation(GameKey.Left, new(texture,3, 4, 0.2f, 4));
    }

    public override void Update(GameTime gameTime)
    {
        _camera.Follow(Position);
        var keyInput = GetKeyInput();
        if (!keyInput.Any())
        {
            _anims.Update(gameTime, new());
            return;
        }

        if (keyInput.Contains(GameKey.Left))
            _anims.Update(gameTime, GameKey.Left);
        else if (keyInput.Contains(GameKey.Right))
            _anims.Update(gameTime, GameKey.Right);
        else if (keyInput.Contains(GameKey.Up))
            _anims.Update(gameTime, GameKey.Up);
        else if (keyInput.Contains(GameKey.Down))
            _anims.Update(gameTime, GameKey.Down);

        var input = new Input()
        {
            PlayerId = Id.ToString(),
            PlayerLocation = new Coordinates()
            {
                X = Position.X,
                Y = Position.Y
            },
            MouseLocation = new Coordinates()
            {
                X = _mousePosition.X,
                Y = _mousePosition.Y
            },
            GameTime = gameTime.ElapsedGameTime.TotalSeconds,
            EventId = Guid.NewGuid().ToString(),
            Source = Source.Player
        };
        input.KeyInput.AddRange(keyInput);
        
        var newLocation = _lastLocation.X != input.PlayerLocation.X || _lastLocation.Y != input.PlayerLocation.Y;
        var newInput = !_lastKeyInput.OrderBy(x => x).SequenceEqual(keyInput.OrderBy(x => x));
        if (!newLocation && !newInput) return;
        
        var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _game.EventTimes.Add(input.EventId, timeStamp);
        string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        Console.WriteLine($"Send {input.EventId} at {timestampWithMs}");
        _producer.Produce(_game.OutputTopic, Id.ToString(), input);
        _lastLocation = input.PlayerLocation;
        _lastKeyInput = input.KeyInput.ToList();
        
        Console.WriteLine($"Mouse: {_mousePosition.X}:{_mousePosition.Y}");

        LocalMovement(input);
    }

    private void LocalMovement(Input value)
    {
        var x = Position.X;
        var y = Position.Y;
        var speed = 100;
        foreach (var input in value.KeyInput)
        {
            switch (input)
            {
                case GameKey.Up:
                    y -= speed * (float) value.GameTime;
                    break;
                case GameKey.Down:
                    y += speed * (float) value.GameTime;
                    break;
                case GameKey.Left:
                    x -= speed * (float) value.GameTime;
                    break;
                case GameKey.Right:
                    x += speed * (float) value.GameTime;
                    break;
            }
        }

        Position = new Vector2(x, y);
    }

    private List<GameKey> GetKeyInput()
    {
        var keyInput = new List<GameKey>();
        var mState = Mouse.GetState();
        if (mState.LeftButton == ButtonState.Pressed &&
            _game.IsActive && _camera.MouseOnScreen(mState.Position.ToVector2()))
        {
            if (!attacking)
            {
                attacking = true;
                keyInput.Add(GameKey.Attack);
                _mousePosition = _camera.MouseInWorld(mState.Position.ToVector2());
            }
        }
        else
        {
            attacking = false;
            _lastKeyInput.Remove(GameKey.Attack);
        }

        var kState = Keyboard.GetState();
        if (kState.IsKeyDown(Keys.W))
            keyInput.Add(GameKey.Up);
        if (kState.IsKeyDown(Keys.S))
            keyInput.Add(GameKey.Down);
        if (kState.IsKeyDown(Keys.A))
            keyInput.Add(GameKey.Left);
        if (kState.IsKeyDown(Keys.D))
            keyInput.Add(GameKey.Right);
        if (kState.IsKeyDown(Keys.Space))
            keyInput.Add(GameKey.Interact);
        return keyInput;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Position.X - (48 / 2), Position.Y - (64 / 2));
        _anims.Draw(spriteBatch, offset);
        //var offset = new Vector2(Position.X - (Texture.Width / 2), Position.Y - (Texture.Height / 2));
        //spriteBatch.Draw(Texture, offset, Color.Green);
    }
}