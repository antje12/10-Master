using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Coin : Sprite
{
    private Animation _anim;
    
    public Coin(Vector2 position, Texture2D texture) : base(position, texture)
    {
        _anim = new Animation(texture, 6, 1, 0.1);
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