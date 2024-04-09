using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Core;

public class UI
{
    private SpriteFont _font;
    private MyGame _game;
    
    public UI(SpriteFont font, MyGame game)
    {
        _font = font;
        _game = game;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_game.Player == null) return;
        var x = _game.Player.Location.X - _game.ScreenWidth / 2;
        var y = _game.Player.Location.Y - _game.ScreenHeight / 2;
        spriteBatch.DrawString(_font, $"Latency: {_game.Latency} ms", new Vector2(x+10, y), Color.Black);
        spriteBatch.DrawString(_font, $"Health: {_game.Player.LifePool} %", new Vector2(x+10, y+20), Color.Black);
        spriteBatch.DrawString(_font, $"Score: {_game.Player.Score} $", new Vector2(x+10, y+40), Color.Black);
    }
}