using ClassLibrary.Classes.Domain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameClient.Core;

namespace GameClient.Sprites;

public class Ocean_S : Ocean, Sprite
{
    public Coordinates Location { get; set; } // Not used
    public Texture2D Texture { get; set; }
    private int Width { get; set; }
    private int Height { get; set; }
    
    private MyGame _game;

    public Ocean_S(MyGame game, Texture2D texture, Ocean o) 
        : base()
    {
        _game = game;
        Texture = texture;
        Width = texture.Width / 1;
        Height = texture.Height / 1;
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int screenWidth = _game.ScreenWidth;
        int screenHeight = _game.ScreenHeight;

        var startX = _game.Player.Location.X - screenWidth / 2 - Width;
        var offsetX = startX % Width;
        startX -= offsetX;

        var startY = _game.Player.Location.Y - screenHeight / 2 - Height;
        var offsetY = startY % Height;
        startY -= offsetY;

        var bgWidth = screenWidth + Width * 2;
        var bgHeight = screenHeight + Height * 2;

        var background = new Rectangle((int) startX, (int) startY, bgWidth, bgHeight);
        for (int y = background.Top; y < background.Bottom; y += Height)
        {
            for (int x = background.Left; x < background.Right; x += Width)
            {
                spriteBatch.Draw(Texture, new Vector2(x, y), Color.White);
            }
        }
    }
}