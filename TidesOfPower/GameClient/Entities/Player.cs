using System;
using System.Collections.Generic;
using ClassLibrary.Classes.Client;
using ClassLibrary.Classes.Data;
using ClassLibrary.Kafka;
using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameClient.Entities;

public class Player : Sprite
{
    private readonly Guid _playerId;
    private readonly Camera _camera;
    private readonly KafkaProducer<Input> _producer;

    public Player(Vector2 position, Texture2D texture, Camera camera, Guid playerId, KafkaProducer<Input> producer)
        : base(position, texture)
    {
        _camera = camera;
        _playerId = playerId;
        _producer = producer;
    }

    public override void Update(GameTime gameTime)
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
        if (kState.IsKeyDown(Keys.Space))
            keyInput.Add(GameKey.Attack);
        if (kState.IsKeyDown(Keys.E))
            keyInput.Add(GameKey.Interact);

        if (keyInput.Count > 0)
        {
            _producer.Produce(KafkaTopic.Input, _playerId.ToString(), new Input()
            {
                PlayerId = _playerId,
                Location = new Coordinates()
                {
                    X = Position.X,
                    Y = Position.Y
                },
                KeyInput = keyInput,
                GameTime = gameTime.ElapsedGameTime.TotalSeconds
            });
        }

        _camera.Follow(Position, Texture, MyGame.screenWidth, MyGame.screenHeight);
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