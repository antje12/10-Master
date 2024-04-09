using ClassLibrary.Classes.Domain;
using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Sprites;

public class Treasure_S : Treasure, Sprite
{
    public Texture2D Texture { get; set; }
    private Animation _anim { get; set; }
    private int Width { get; set; }
    private int Height { get; set; }

    public Treasure_S(Texture2D texture, int framesX, Treasure t) 
        : base(t.Value, t.Id, t.Location)
    {
        Texture = texture;
        _anim = new Animation(texture, framesX, 1, 0.2);
        Width = texture.Width / framesX;
        Height = texture.Height / 1;
    }

    public void Update(GameTime gameTime)
    {
        _anim.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Location.X - Width / 2, Location.Y - Height / 2);
        _anim.Draw(spriteBatch, offset);
    }
}