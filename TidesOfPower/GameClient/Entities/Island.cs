using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public class Island : Sprite
{
    private List<Rectangle> _subTexture = new();
    // 0 1 2
    // 3 4 5
    // 6 7 8
    
    private int _fromX;
    private int _toX;
    private int _fromY;
    private int _toY;
    
    public Island(Vector2 position, Texture2D texture) : base(position, texture)
    {
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
        
        _fromX = 64;
        _toX = 64 + (64 * 5);
        _fromY = 64;
        _toY = 64 + (64 * 5);
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        DrawCorners(spriteBatch);
        DrawBorders(spriteBatch);
        DrawCenter(spriteBatch);
    }

    private void DrawCorners(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, new Vector2(_fromX, _fromY), _subTexture[0], Color.White);
        spriteBatch.Draw(Texture, new Vector2(_toX-64, _fromY), _subTexture[2], Color.White);
        spriteBatch.Draw(Texture, new Vector2(_fromX, _toY-64), _subTexture[6], Color.White);
        spriteBatch.Draw(Texture, new Vector2(_toX-64, _toY-64), _subTexture[8], Color.White);
    }

    private void DrawBorders(SpriteBatch spriteBatch)
    {
        var startX = _fromX+64;
        var endX = _toX-64;
        for (int x = startX; x < endX; x += _subTexture[0].Width)
        {
            // North
            spriteBatch.Draw(Texture, new Vector2(x, _fromY), _subTexture[1], Color.White);
            // South
            spriteBatch.Draw(Texture, new Vector2(x, _toY-64), _subTexture[7], Color.White);
        }
        
        var startY = _fromY+64;
        var endY = _toY-64;
        for (int y = startY; y < endY; y += _subTexture[0].Height)
        {
            // East
            spriteBatch.Draw(Texture, new Vector2(_fromX, y), _subTexture[3], Color.White);
            // West
            spriteBatch.Draw(Texture, new Vector2(_toX-64, y), _subTexture[5], Color.White);
        }
    }
    
    
    private void DrawCenter(SpriteBatch spriteBatch)
    {
        var startX = _fromX+64;
        var endX = _toX-64;
        var startY = _fromY+64;
        var endY = _toY-64;
        
        for (int y = startY; y < endY; y += _subTexture[0].Height)
        {
            for (int x = startX; x < endX; x += _subTexture[0].Width)
            {
                spriteBatch.Draw(Texture, new Vector2(x, y), _subTexture[4], Color.White);
            }
        }
    }
}