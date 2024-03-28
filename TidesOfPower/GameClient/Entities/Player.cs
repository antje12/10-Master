using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.GameLogic;
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

        var msgOut = new Input()
        {
            AgentId = Id.ToString(),
            AgentLocation = new Coordinates()
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
        msgOut.KeyInput.AddRange(keyInput);
        
        var newLocation = _lastLocation.X != msgOut.AgentLocation.X || _lastLocation.Y != msgOut.AgentLocation.Y;
        var newInput = !_lastKeyInput.OrderBy(x => x).SequenceEqual(keyInput.OrderBy(x => x));
        if (!newLocation && !newInput) return;
        
        var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _game.EventTimes.Add(msgOut.EventId, timeStamp);
        string timestampWithMs = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss.ffffff");
        Console.WriteLine($"Send {msgOut.EventId} at {timestampWithMs}");
        _producer.Produce(_game.OutputTopic, Id.ToString(), msgOut);
        _lastLocation = msgOut.AgentLocation;
        _lastKeyInput = msgOut.KeyInput.ToList();
        
        Console.WriteLine($"Mouse: {_mousePosition.X}:{_mousePosition.Y}");

        LocalMovement(keyInput, msgOut.GameTime);
    }

    private void LocalMovement(List<GameKey> keyInput, double gameTime)
    {
        Move.Avatar(Position.X, Position.Y, keyInput, gameTime, out float toX, out float toY);
        var to = new Vector2(toX, toY);
        if (IsLocationFree(to))
            Position = to;
    }

    private bool IsLocationFree(Vector2 to)
    {
        foreach (var sprite in _game.LocalState)
        {
            var w1 = 25;
            var w2 =
                sprite is Projectile ? 5 :
                sprite is Agent ? 25 : 0;

            if (Collide.Circle(to.X, to.Y, w1, 
                    sprite.Position.X, sprite.Position.Y, w2))
            {
                if (sprite is Agent)
                {
                    return false;
                }
            }
        }

        return true;
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