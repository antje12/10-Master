using GameClient.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Core;

public class UI
{
    private SpriteFont _font;
    
    public UI(SpriteFont font)
    {
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch, Player player)
    {
        var x = player.Position.X - MyGame.screenWidth / 2;
        var y = player.Position.Y - MyGame.screenHeight / 2;
        spriteBatch.DrawString(_font, $"Latency: {player.Latency}", new Vector2(x+10, y), Color.Black);
    }
}