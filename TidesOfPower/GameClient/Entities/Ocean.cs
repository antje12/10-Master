using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Ocean : Sprite
{
    private Player _player;
    private MyGame _game;

    public Ocean(Vector2 position, Texture2D texture, Player player, MyGame game) : base(position, texture)
    {
        _player = player;
        _game = game;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        int screenWidth = _game.ScreenWidth;
        int screenHeight = _game.ScreenHeight;

        var startX = _player.Position.X - screenWidth / 2 - Texture.Width;
        var offsetX = startX % Texture.Width;
        startX -= offsetX;

        var startY = _player.Position.Y - screenHeight / 2 - Texture.Height;
        var offsetY = startY % Texture.Height;
        startY -= offsetY;

        var bgWidth = screenWidth + Texture.Width * 2;
        var bgHeight = screenHeight + Texture.Height * 2;

        var background = new Rectangle((int) startX, (int) startY, bgWidth, bgHeight);
        for (int y = background.Top; y < background.Bottom; y += Texture.Height)
        {
            for (int x = background.Left; x < background.Right; x += Texture.Width)
            {
                spriteBatch.Draw(Texture, new Vector2(x, y), Color.White);
            }
        }
    }
}