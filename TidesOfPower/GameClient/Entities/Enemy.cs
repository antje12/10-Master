using System;
using ClassLibrary.Classes.Data;
using ClassLibrary.Messages.Protobuf;
using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Enemy : Agent
{
    private readonly AnimationManager _anims = new();
    private Vector2 LastPosition;
    private DateTime LastUpdate;

    public Enemy(Guid agentId, Vector2 position, Texture2D texture) : base(agentId, position, texture)
    {
        _anims.AddAnimation(GameKey.Up, new(texture, 3, 4, 0.2f, 1));
        _anims.AddAnimation(GameKey.Right, new(texture, 3, 4, 0.2f, 2));
        _anims.AddAnimation(GameKey.Down, new(texture, 3, 4, 0.2f, 3));
        _anims.AddAnimation(GameKey.Left, new(texture,3, 4, 0.2f, 4));
        LastPosition = position;
        LastUpdate = DateTime.Now;
    }

    public void SetPosition(Vector2 newPosition)
    {
        LastPosition = Position;
        Position = newPosition;
        LastUpdate = DateTime.Now;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (Position.X < LastPosition.X)
            _anims.Update(gameTime, GameKey.Left);
        else if (Position.X > LastPosition.X)
            _anims.Update(gameTime, GameKey.Right);
        else if (Position.Y < LastPosition.Y)
            _anims.Update(gameTime, GameKey.Up);
        else if (Position.Y > LastPosition.Y)
            _anims.Update(gameTime, GameKey.Down);

        TimeSpan timeSpan = DateTime.Now - LastUpdate;
        if (timeSpan.Milliseconds > 100)
            _anims.Update(gameTime, new());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Position.X - (48 / 2), Position.Y - (64 / 2));
        _anims.Draw(spriteBatch, offset);
        //var offset = new Vector2(Position.X - (Texture.Width / 2), Position.Y - (Texture.Height / 2));
        //spriteBatch.Draw(Texture, offset, Color.Red);
    }
}