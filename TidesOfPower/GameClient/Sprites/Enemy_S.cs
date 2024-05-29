using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Domain;
using ClassLibrary.Messages.Protobuf;
using GameClient.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Sprites;

public class Enemy_S : Enemy, Sprite
{
    public Texture2D Texture { get; set; }
    private int Width;
    private int Height;

    private Texture2D s_texture;
    private float s_rotation;
    private int s_width;
    private int s_height;
    
    private MyGame _game;
    private readonly AnimationManager _anims = new();
    private Coordinates LastLocation;
    private DateTime LastUpdate;

    public Enemy_S(MyGame game, Texture2D texture, Texture2D texture2, Enemy e) 
        : base(e.Id, e.Location, e.LifePool, e.WalkingSpeed)
    {
        Texture = texture;
        Width = texture.Width / 3;
        Height = texture.Height / 4;
        
        s_texture = texture2;
        s_rotation = 0f;
        s_width = s_texture.Width / 1;
        s_height = s_texture.Height / 1;
        
        _game = game;
        LastLocation = Location;
        LastUpdate = DateTime.UtcNow;
        
        _anims.AddAnimation(GameKey.Up, new(texture, 3, 4, 0.2f, 1));
        _anims.AddAnimation(GameKey.Right, new(texture, 3, 4, 0.2f, 2));
        _anims.AddAnimation(GameKey.Down, new(texture, 3, 4, 0.2f, 3));
        _anims.AddAnimation(GameKey.Left, new(texture,3, 4, 0.2f, 4));
    }

    public void SetLocation(Coordinates newLocation)
    {
        LastLocation = Location;
        Location = newLocation;
        LastUpdate = DateTime.UtcNow;
    }
    
    public void Update(GameTime gameTime)
    {
        if (Location.X < LastLocation.X)
            _anims.Update(gameTime, GameKey.Left);
        else if (Location.X > LastLocation.X)
            _anims.Update(gameTime, GameKey.Right);
        else if (Location.Y < LastLocation.Y)
            _anims.Update(gameTime, GameKey.Up);
        else if (Location.Y > LastLocation.Y)
            _anims.Update(gameTime, GameKey.Down);

        TimeSpan timeSpan = DateTime.UtcNow - LastUpdate;
        if (timeSpan.Milliseconds > 100)
            _anims.Update(gameTime, new()); // reset animation
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        var keys = new List<GameKey>();
        if (Location.X < LastLocation.X)
            keys.Add(GameKey.Left);
        if (Location.X > LastLocation.X)
            keys.Add(GameKey.Right);
        if (Location.Y < LastLocation.Y)
            keys.Add(GameKey.Up);
        if (Location.Y > LastLocation.Y)
            keys.Add(GameKey.Down);
        
        if (keys.Contains(GameKey.Up) && keys.Contains(GameKey.Right))
            s_rotation = 5 * MathHelper.PiOver4;
        else if (keys.Contains(GameKey.Up) && keys.Contains(GameKey.Left))
            s_rotation = 3 * MathHelper.PiOver4;
        else if (keys.Contains(GameKey.Down) && keys.Contains(GameKey.Right))
            s_rotation = 7 * MathHelper.PiOver4;
        else if (keys.Contains(GameKey.Down) && keys.Contains(GameKey.Left))
            s_rotation = 1 * MathHelper.PiOver4;
        else if (keys.Contains(GameKey.Up))
            s_rotation = 4 * MathHelper.PiOver4;
        else if (keys.Contains(GameKey.Down))
            s_rotation = 8 * MathHelper.PiOver4;
        else if (keys.Contains(GameKey.Left))
            s_rotation = 2 * MathHelper.PiOver4;
        else if (keys.Contains(GameKey.Right))
            s_rotation = 6 * MathHelper.PiOver4;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var isOnIsland = _game.LocalState.OfType<Island_S>().Any(island => island.IsOn(Location));
        if (isOnIsland)
        {
            var offset = new Vector2(Location.X - Width / 2, Location.Y - Height / 2);
            _anims.Draw(spriteBatch, offset);
        }
        else
        {
            Vector2 origin = new Vector2(s_width / 2, s_height / 2);
            spriteBatch.Draw(s_texture, new Vector2(Location.X, Location.Y),
                null, Color.White, s_rotation, origin, 1.0f, SpriteEffects.None, 0f);
        }
    }
}