using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Island : Sprite
{
    public Island(Vector2 position, Texture2D texture) : base(position, texture)
    {
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Position.X - (Texture.Width / 2), Position.Y - (Texture.Height / 2));
        //Rectangle island = new Rectangle(screenWidth / 2, screenHeight / 2, 64, 64);
        //_spriteBatch.Draw(islandTexture, island, Color.White);
        spriteBatch.Draw(Texture, offset, Color.White);
    }
}