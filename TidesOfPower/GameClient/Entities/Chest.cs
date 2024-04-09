using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Chest : Sprite
{
    private Animation _anim;
    
    public Chest(Vector2 position, Texture2D texture) : base(position, texture)
    {
        _anim = new Animation(texture, 4, 1, 0.2);
    }

    public override void Update(GameTime gameTime)
    {
        _anim.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        _anim.Draw(spriteBatch, Position);
    }
}