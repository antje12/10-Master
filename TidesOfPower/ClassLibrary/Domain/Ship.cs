﻿using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Ship : Entity
{
    [BsonElement("life-pool")] public int LifePool { get; set; }
    public static readonly int TypeRadius = 50;

    public Ship(int lifePool, Guid id, Coordinates location) 
        : base(id, location, EntityType.Ship, TypeRadius)
    {
        LifePool = lifePool;
    }
    
    public void Embark()
    {
    }
    
    public void DisEmbark()
    {
    }
}