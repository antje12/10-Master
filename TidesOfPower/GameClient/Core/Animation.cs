using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Core;

public class Animation
{
    private Texture2D _texture;
    private List<Rectangle> _sourceRectangles = new();
    private int _frames;
    private int _frame;
    private double _frameTime;
    private double _frameTimeLeft;
    private bool _active = true;

    public Animation(Texture2D texture, int framesX, int framesY, double frameTime, int row = 1)
    {
        _texture = texture;
        _frameTime = frameTime;
        _frameTimeLeft = frameTime;
        _frames = framesX;
        
        var frameWidth = _texture.Width / framesX;
        var frameHeight = _texture.Height / framesY;

        for (int i = 0; i < _frames; i++)
        {
            _sourceRectangles.Add(new(i * frameWidth, (row - 1) * frameHeight, frameWidth, frameHeight));
        }
    }
    
    public void Stop()
    {
        _active = false;
    }

    public void Start()
    {
        _active = true;
    }

    public void Reset()
    {
        _frame = 0;
        _frameTimeLeft = _frameTime;
    }

    public void Update(GameTime gameTime)
    {
        if (!_active) return;

        _frameTimeLeft -= gameTime.ElapsedGameTime.TotalSeconds;

        if (_frameTimeLeft <= 0)
        {
            _frameTimeLeft += _frameTime;
            _frame = (_frame + 1) % _frames;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos)
    {
        spriteBatch.Draw(_texture, pos, _sourceRectangles[_frame], Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
    }
}