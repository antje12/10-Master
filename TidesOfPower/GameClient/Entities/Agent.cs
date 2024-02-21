using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Entities;

public abstract class Agent : Sprite
{
    public Guid _agentId;

    protected Agent(Guid agentId, Vector2 position, Texture2D texture) : base(position, texture)
    {
        _agentId = agentId;
    }
}