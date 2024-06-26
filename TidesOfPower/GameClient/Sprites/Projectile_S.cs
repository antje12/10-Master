﻿using ClassLibrary.Domain;
using GameClient.Core;
using ClassLibrary.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Sprites;

public class Projectile_S : Projectile, Sprite
{
    public Texture2D Texture { get; set; }
    private int Width;
    private int Height;

    public Projectile_S(Texture2D texture, Projectile p)
        : base(p.Direction, p.FlyingSpeed, p.TimeToLive, p.Damage, p.Id, p.Location)
    {
        Texture = texture;
        Width = texture.Width / 1;
        Height = texture.Height / 1;
    }

    public void Update(GameTime gameTime)
    {
        LocalMovement(gameTime.ElapsedGameTime.TotalSeconds);
    }

    private void LocalMovement(double gameTime)
    {
        Move.Projectile(
            Location.X, Location.Y, Direction.X, Direction.Y, gameTime,
            out double time, out float toX, out float toY);
        Location = new Coordinates(toX, toY);
        TimeToLive -= time;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Location.X - Width / 2, Location.Y - Height / 2);
        spriteBatch.Draw(Texture, offset, Color.White);
    }
}