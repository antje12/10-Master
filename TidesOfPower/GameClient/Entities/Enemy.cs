using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Enemy : Agent
{
    public Enemy(Guid agentId, Vector2 position, Texture2D texture) : base(agentId, position, texture)
    {
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Position.X - (Texture.Width / 2), Position.Y - (Texture.Height / 2));
        spriteBatch.Draw(Texture, offset, Color.Red);
    }
}