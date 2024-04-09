using System;
using ClassLibrary.Classes.Domain;
using ClassLibrary.Messages.Protobuf;
using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Coordinates = ClassLibrary.Classes.Domain.Coordinates;

namespace GameClient.Sprites;

public class Enemy_S : Enemy, Sprite
{
    public Texture2D Texture { get; set; }
    private int Width { get; set; }
    private int Height { get; set; }
    
    private readonly AnimationManager _anims = new();
    private Coordinates LastPosition;
    private DateTime LastUpdate;

    public Enemy_S(Texture2D texture, Enemy e) 
        : base(e.Id, e.Location, e.LifePool, e.WalkingSpeed)
    {
        Texture = texture;
        Width = texture.Width / 3;
        Height = texture.Height / 4;
        _anims.AddAnimation(GameKey.Up, new(texture, 3, 4, 0.2f, 1));
        _anims.AddAnimation(GameKey.Right, new(texture, 3, 4, 0.2f, 2));
        _anims.AddAnimation(GameKey.Down, new(texture, 3, 4, 0.2f, 3));
        _anims.AddAnimation(GameKey.Left, new(texture,3, 4, 0.2f, 4));
        LastPosition = Location;
        LastUpdate = DateTime.Now;
    }

    public void SetPosition(Coordinates newPosition)
    {
        LastPosition = Location;
        Location = newPosition;
        LastUpdate = DateTime.Now;
    }
    
    public void Update(GameTime gameTime)
    {
        if (Location.X < LastPosition.X)
            _anims.Update(gameTime, GameKey.Left);
        else if (Location.X > LastPosition.X)
            _anims.Update(gameTime, GameKey.Right);
        else if (Location.Y < LastPosition.Y)
            _anims.Update(gameTime, GameKey.Up);
        else if (Location.Y > LastPosition.Y)
            _anims.Update(gameTime, GameKey.Down);

        TimeSpan timeSpan = DateTime.Now - LastUpdate;
        if (timeSpan.Milliseconds > 100)
            _anims.Update(gameTime, new());
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var offset = new Vector2(Location.X - Width / 2, Location.Y - Height / 2);
        _anims.Draw(spriteBatch, offset);
    }
}