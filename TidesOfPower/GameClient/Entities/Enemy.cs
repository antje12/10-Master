﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Enemy : Base
{
    public Enemy(Vector2 position, Texture2D texture) : base(position, texture)
    {
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.Red, 0f,
            new Vector2(Texture.Width / 2, Texture.Height / 2), Vector2.One,
            SpriteEffects.None,
            0f);
    }
}