using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Ocean : Sprite
{
    private readonly Player _player;
    
    public Ocean(Vector2 position, Texture2D texture, Player player) : base(position, texture)
    {
        _player = player;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        int screenWidth = MyGame.screenWidth;
        int screenHeight = MyGame.screenHeight;
        
        var startX = _player.Position.X - screenWidth / 2 - Texture.Width;
        var offsetX = startX % Texture.Width;
        startX -= offsetX;

        var startY = _player.Position.Y - screenHeight / 2 - Texture.Height;
        var offsetY = startY % Texture.Height;
        startY -= offsetY;

        var bgWidth = screenWidth + Texture.Width * 2;
        var bgHeight = screenHeight + Texture.Height * 2;

        Rectangle background = new Rectangle((int) startX, (int) startY, bgWidth, bgHeight);
        //_spriteBatch.Draw(oceanTexture, background, Color.White);
        // Draw the repeating texture using a loop to cover the entire destination rectangle
        for (int y = background.Top; y < background.Bottom; y += Texture.Height)
        {
            for (int x = background.Left; x < background.Right; x += Texture.Width)
            {
                spriteBatch.Draw(Texture, new Vector2(x, y), Color.White);
            }
        }
    }
}