using GameClient.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Core;

public class UI
{
    private SpriteFont _font;
    private Player _player;
    private MyGame _game;
    
    public UI(SpriteFont font, Player player, MyGame game)
    {
        _font = font;
        _player = player;
        _game = game;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_player == null) return;
        var x = _player.Position.X - _game.ScreenWidth / 2;
        var y = _player.Position.Y - _game.ScreenHeight / 2;
        spriteBatch.DrawString(_font, $"Latency: {_player.Latency} ms", new Vector2(x+10, y), Color.Black);
        spriteBatch.DrawString(_font, $"Health: {_player.Health} %", new Vector2(x+10, y+20), Color.Black);
        spriteBatch.DrawString(_font, $"Score: {_player.Score} $", new Vector2(x+10, y+40), Color.Black);
    }
}