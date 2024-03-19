﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameClient.Entities;

public class Ship : Sprite
{
    private float rotation;

    public Ship(Vector2 position, Texture2D texture) : base(position, texture)
    {
        rotation = 0f;
    }

    public override void Update(GameTime gameTime)
    {
        var kState = Keyboard.GetState();

        if (kState.IsKeyDown(Keys.W) && kState.IsKeyDown(Keys.D))
            rotation = 5 * MathHelper.PiOver4;
        else if (kState.IsKeyDown(Keys.W) && kState.IsKeyDown(Keys.A))
            rotation = 3 * MathHelper.PiOver4;
        else if (kState.IsKeyDown(Keys.S) && kState.IsKeyDown(Keys.D))
            rotation = 7 * MathHelper.PiOver4;
        else if (kState.IsKeyDown(Keys.S) && kState.IsKeyDown(Keys.A))
            rotation = 1 * MathHelper.PiOver4;
        else if (kState.IsKeyDown(Keys.W))
            rotation = 4 * MathHelper.PiOver4;
        else if (kState.IsKeyDown(Keys.S))
            rotation = 8 * MathHelper.PiOver4;
        else if (kState.IsKeyDown(Keys.A))
            rotation = 2 * MathHelper.PiOver4;
        else if (kState.IsKeyDown(Keys.D))
            rotation = 6 * MathHelper.PiOver4;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        spriteBatch.Draw(Texture, Position, null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0f);
    }
}