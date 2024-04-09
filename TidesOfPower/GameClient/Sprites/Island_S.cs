using System.Collections.Generic;
using ClassLibrary.Classes.Domain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameClient.Core;

namespace GameClient.Sprites;

public class Island_S : Island, Sprite
{
    public Coordinates Location { get; set; } // Not used
    public Texture2D Texture { get; set; }
    
    private List<Rectangle> _subTexture = new();
    // 0 1 2
    // 3 4 5
    // 6 7 8
    
    public Island_S(Texture2D texture, Island i) 
        : base(i.FromX, i.ToX, i.FromY, i.ToY)
    {
        Texture = texture;
        
        var framesX = 3;
        var framesY = 3;
        
        var frameWidth = Texture.Width / framesX;
        var frameHeight = Texture.Height / framesY;

        for (int y = 0; y < framesY; y++)
        {
            for (int x = 0; x < framesX; x++)
            {
                _subTexture.Add(new(x * frameWidth, y * frameHeight, frameWidth, frameHeight));
            }
        }
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawCorners(spriteBatch);
        DrawBorders(spriteBatch);
        DrawCenter(spriteBatch);
    }

    private void DrawCorners(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, new Vector2(FromX, FromY), _subTexture[0], Color.White);
        spriteBatch.Draw(Texture, new Vector2(ToX-64, FromY), _subTexture[2], Color.White);
        spriteBatch.Draw(Texture, new Vector2(FromX, ToY-64), _subTexture[6], Color.White);
        spriteBatch.Draw(Texture, new Vector2(ToX-64, ToY-64), _subTexture[8], Color.White);
    }

    private void DrawBorders(SpriteBatch spriteBatch)
    {
        var startX = FromX+64;
        var endX = ToX-64;
        for (int x = startX; x < endX; x += _subTexture[0].Width)
        {
            // North
            spriteBatch.Draw(Texture, new Vector2(x, FromY), _subTexture[1], Color.White);
            // South
            spriteBatch.Draw(Texture, new Vector2(x, ToY-64), _subTexture[7], Color.White);
        }
        
        var startY = FromY+64;
        var endY = ToY-64;
        for (int y = startY; y < endY; y += _subTexture[0].Height)
        {
            // East
            spriteBatch.Draw(Texture, new Vector2(FromX, y), _subTexture[3], Color.White);
            // West
            spriteBatch.Draw(Texture, new Vector2(ToX-64, y), _subTexture[5], Color.White);
        }
    }
    
    private void DrawCenter(SpriteBatch spriteBatch)
    {
        var startX = FromX+64;
        var endX = ToX-64;
        var startY = FromY+64;
        var endY = ToY-64;
        
        for (int y = startY; y < endY; y += _subTexture[0].Height)
        {
            for (int x = startX; x < endX; x += _subTexture[0].Width)
            {
                spriteBatch.Draw(Texture, new Vector2(x, y), _subTexture[4], Color.White);
            }
        }
    }
}