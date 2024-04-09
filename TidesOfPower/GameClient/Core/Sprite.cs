using ClassLibrary.Classes.Domain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Core;

public interface Sprite
{
    public Coordinates Location { get; set; }
    protected Texture2D Texture { get; set; }
    public void Update(GameTime gameTime);
    public void Draw(SpriteBatch spriteBatch);
}