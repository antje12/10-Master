using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Core;

public class UI
{
    private SpriteFont _font;
    private Camera _camera;
    private MyGame _game;
    
    public UI(SpriteFont font, Camera camera, MyGame game)
    {
        _font = font;
        _camera = camera;
        _game = game;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_game.Player == null) return;
        var windowCorner = _camera.MouseInWorld(new Vector2(0,0));
        var x = windowCorner.X;
        var y = windowCorner.Y;
        spriteBatch.DrawString(_font, $"Latency: {_game.Latency} ms", new Vector2(x+10, y), Color.Black);
        spriteBatch.DrawString(_font, $"Health: {_game.Player.LifePool} %", new Vector2(x+10, y+20), Color.Black);
        spriteBatch.DrawString(_font, $"Score: {_game.Player.Score} $", new Vector2(x+10, y+40), Color.Black);
    }
}