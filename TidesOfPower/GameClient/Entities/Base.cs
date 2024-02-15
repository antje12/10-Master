using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public abstract class Base
{
    public Vector2 Position { get; set; }
    protected Texture2D Texture { get; set; }

    protected Base(Vector2 position, Texture2D texture)
    {
        Position = position;
        Texture = texture;
    }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}