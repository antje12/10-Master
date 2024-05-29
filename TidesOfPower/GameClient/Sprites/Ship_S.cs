using ClassLibrary.Domain;
using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameClient.Sprites;

public class Ship_S : Ship, Sprite
{
    public Texture2D Texture { get; set; }
    private float rotation;
    private int Width;
    private int Height;

    public Ship_S(Texture2D texture, Ship s) 
        : base(s.LifePool, s.Id, s.Location)
    {
        Texture = texture;
        rotation = 0f;
        Width = texture.Width / 1;
        Height = texture.Height / 1;
    }

    public void Update(GameTime gameTime)
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

    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 origin = new Vector2(Width / 2, Height / 2);
        spriteBatch.Draw(Texture, new Vector2(Location.X, Location.Y),
            null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0f);
    }
}