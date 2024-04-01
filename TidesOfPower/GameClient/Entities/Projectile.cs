using System;
using ClassLibrary.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Projectile : Sprite
{
    public Guid Id;
    private Vector2 _direction;
    public double TimeToLive;

    public Projectile(Guid id, Vector2 position, Vector2 direction, Texture2D texture) : base(position, texture)
    {
        Id = id;
        _direction = direction;
        TimeToLive = 100;
    }

    public override void Update(GameTime gameTime)
    {
        LocalMovement(gameTime.ElapsedGameTime.TotalSeconds);
    }

    private void LocalMovement(double gameTime)
    {
        Move.Projectile(Position.X, Position.Y, _direction.X,
            _direction.Y, gameTime,
            out double time, out float toX, out float toY);
        Position = new Vector2(toX, toY);
        TimeToLive -= time;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Position.X - (Texture.Width / 2), Position.Y - (Texture.Height / 2));
        spriteBatch.Draw(Texture, offset, Color.White);
    }
}