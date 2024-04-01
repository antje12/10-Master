using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public abstract class Sprite
{
    public Vector2 Position { get; set; }
    protected Texture2D Texture { get; set; }

    protected Sprite(Vector2 position, Texture2D texture)
    {
        Position = position;
        Texture = texture;
    }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
}