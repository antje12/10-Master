using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public abstract class Agent : Sprite
{
    public Guid Id;

    protected Agent(Guid id, Vector2 position, Texture2D texture) : base(position, texture)
    {
        Id = id;
    }
}