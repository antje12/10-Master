using System;
using GameClient.Core;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Classes.Domain;
using ClassLibrary.GameLogic;
using ClassLibrary.Kafka;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClassLibrary.Messages.Protobuf;
using Coordinates = ClassLibrary.Messages.Protobuf.Coordinates;

namespace GameClient.Sprites;

public class Player_S : Player, Sprite
{
    public Texture2D Texture { get; set; }
    private int Width { get; set; }
    private int Height { get; set; }
    
    private MyGame _game;
    private Camera _camera;
    private Vector2 _mouseLocation;
    private ProtoKafkaProducer<Input> _producer;
    private AnimationManager _anims = new();

    private Coordinates _lastLocation;
    private List<GameKey> _lastKeyInput;
    private bool _attacking;
    private bool _interacting;

    public Player_S(MyGame game, Texture2D texture, Camera camera, ProtoKafkaProducer<Input> producer, Player p) 
        : base(p.Name, p.Score, p.Id, p.Location, p.LifePool, p.WalkingSpeed)
    {
        Texture = texture;
        Width = texture.Width / 3;
        Height = texture.Height / 4;
        
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

    public void Update(GameTime gameTime)
    {
        _camera.Follow(Location);
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
                X = Location.X,
                Y = Location.Y
            },
            MouseLocation = new Coordinates()
            {
                X = _mouseLocation.X,
                Y = _mouseLocation.Y
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
        
        LocalMovement(keyInput, msgOut.GameTime);
    }

    private void LocalMovement(List<GameKey> keyInput, double gameTime)
    {
        Move.Avatar(Location.X, Location.Y, keyInput, gameTime, out float toX, out float toY);
        var to = new ClassLibrary.Classes.Domain.Coordinates(toX,toY);
        if (IsLocationFree(to))
            Location = to;
    }

    private bool IsLocationFree(ClassLibrary.Classes.Domain.Coordinates to)
    {
        var entities = _game.LocalState.Where(x => x is Entity);
        foreach (var entity in entities)
        {
            var w1 = 25;
            var w2 =
                entity is Projectile_S ? 5 :
                entity is Enemy_S ? 25 : 0;

            if (Collide.Circle(to.X, to.Y, w1, 
                    entity.Location.X, entity.Location.Y, w2))
            {
                if (entity is Enemy_S)
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
        
        var kState = Keyboard.GetState();
        if (kState.IsKeyDown(Keys.W))
            keyInput.Add(GameKey.Up);
        if (kState.IsKeyDown(Keys.S))
            keyInput.Add(GameKey.Down);
        if (kState.IsKeyDown(Keys.A))
            keyInput.Add(GameKey.Left);
        if (kState.IsKeyDown(Keys.D))
            keyInput.Add(GameKey.Right);
        
        var mState = Mouse.GetState();
        if (mState.LeftButton == ButtonState.Pressed &&
            _game.IsActive && _camera.MouseOnScreen(mState.Position.ToVector2()))
        {
            if (!_attacking)
            {
                _attacking = true;
                keyInput.Add(GameKey.Attack);
                _mouseLocation = _camera.MouseInWorld(mState.Position.ToVector2());
            }
        }
        else
        {
            _attacking = false;
            _lastKeyInput.Remove(GameKey.Attack);
        }
        
        if (kState.IsKeyDown(Keys.Space) &&
            _game.IsActive && _camera.MouseOnScreen(mState.Position.ToVector2()))
        {
            if (!_interacting)
            {
                _interacting = true;
                keyInput.Add(GameKey.Interact);
            }
        }
        else
        {
            _interacting = false;
            _lastKeyInput.Remove(GameKey.Interact);
        }
        
        return keyInput;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Location.X - Width / 2, Location.Y - Height / 2);
        _anims.Draw(spriteBatch, offset);
    }
}