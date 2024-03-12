﻿using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Kafka;
using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClassLibrary.Messages.Protobuf;
using Coordinates = ClassLibrary.Messages.Protobuf.Coordinates;
using GameKey = ClassLibrary.Messages.Protobuf.GameKey;

namespace GameClient.Entities;

public class Player : Agent
{
    private MyGame _game;
    
    public Vector2 MousePosition { get; set; }
    private readonly Camera _camera;
    private readonly ProtoKafkaProducer<Input> _producer;

    private Coordinates _lastLocation;
    private List<GameKey> _lastKeyInput;

    private bool attacking = false;

    public Player(MyGame game, Guid agentId, Vector2 position, Texture2D texture, Camera camera, ProtoKafkaProducer<Input> producer)
        : base(agentId, position, texture)
    {
        _game = game;
        _camera = camera;
        _producer = producer;
        _lastLocation = new Coordinates();
        _lastKeyInput = new List<GameKey>();
    }

    public override void Update(GameTime gameTime)
    {
        var keyInput = new List<GameKey>();
        
        var mState = Mouse.GetState();
        if (mState.RightButton == ButtonState.Pressed && MouseOnScreen(mState.Position.ToVector2()))
        {
            if (!attacking)
            {
                attacking = true;
                keyInput.Add(GameKey.Attack);
                MousePosition = MouseInWorld(mState.Position.ToVector2(), _camera);
                //Console.WriteLine($"Mouse clicked at {MousePosition.X}:{MousePosition.Y}");
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
        //if (kState.IsKeyDown(Keys.Space))
        //{
        //    if (!attacking)
        //    {
        //        attacking = true;
        //        keyInput.Add(GameKey.Attack);
        //    }
        //}
        //else
        //{
        //    attacking = false;
        //    _lastKeyInput.Remove(GameKey.Attack);
        //}
        if (kState.IsKeyDown(Keys.Space))
            keyInput.Add(GameKey.Interact);

        if (keyInput.Count > 0)
        {
            var input = new Input()
            {
                PlayerId = _agentId.ToString(),
                PlayerLocation = new Coordinates()
                {
                    X = Position.X,
                    Y = Position.Y
                },
                MouseLocation = new Coordinates()
                {
                    X = MousePosition.X,
                    Y = MousePosition.Y
                },
                GameTime = gameTime.ElapsedGameTime.TotalSeconds,
                EventId = Guid.NewGuid().ToString()
            };
            input.KeyInput.AddRange(keyInput);

            var newLocation = _lastLocation.X != input.PlayerLocation.X || _lastLocation.Y != input.PlayerLocation.Y;
            var newInput = !_lastKeyInput.OrderBy(x => x).SequenceEqual(keyInput.OrderBy(x => x));

            if (keyInput.Any() && (newLocation || newInput))
            {
                var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                _game.dict.Add(input.EventId, timeStamp);
                _producer.Produce(MyGame.OutputTopic, _agentId.ToString(), input);
                _lastLocation = input.PlayerLocation;
                _lastKeyInput = input.KeyInput.ToList();
            }
        }

        _camera.Follow(Position, Texture, MyGame.screenWidth, MyGame.screenHeight);
    }

    private bool MouseOnScreen(Vector2 mouse)
    {
        return 0 <= mouse.X && mouse.X <= MyGame.screenWidth &&
               0 <= mouse.Y && mouse.Y <= MyGame.screenHeight;
    }
    
    public Vector2 MouseInWorld(Vector2 screenPosition, Camera camera)
    {
        Matrix inverseTransform = Matrix.Invert(camera.Transform);
        Vector3 screenPosition3 = new Vector3(screenPosition, 0);
        Vector3 worldPosition = Vector3.Transform(screenPosition3, inverseTransform);
        return new Vector2(worldPosition.X, worldPosition.Y);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Position.X - (Texture.Width / 2), Position.Y - (Texture.Height / 2));
        spriteBatch.Draw(Texture, offset, Color.Green);
        //spriteBatch.Draw(Texture, Position, null, Color.Green, 0f,
        //    new Vector2(Texture.Width / 2, Texture.Height / 2), Vector2.One,
        //    SpriteEffects.None,
        //    0f);
    }
}