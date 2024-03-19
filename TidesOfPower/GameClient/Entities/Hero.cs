using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Hero : Sprite
{
    private readonly AnimationManager _anims = new();
    
    public Hero(Vector2 position, Texture2D texture) : base(position, texture)
    {
        _anims.AddAnimation(1, new(texture, 3, 4, 0.2f, 1));
        _anims.AddAnimation(2, new(texture, 3, 4, 0.2f, 2));
        _anims.AddAnimation(3, new(texture, 3, 4, 0.2f, 3));
        _anims.AddAnimation(4, new(texture,3, 4, 0.2f, 4));
    }

    public override void Update(GameTime gameTime)
    {
        _anims.Update(gameTime, 2);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Position.X - (48 / 2), Position.Y - (64 / 2));
        _anims.Draw(spriteBatch, offset);
    }
}