﻿using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Ship : Entity
{
    [BsonElement("life-pool")] public int LifePool { get; set; }

    public Ship()
    {
        Type = EntityType.Ship;
    }
    
    public void Embark()
    {
    }
    
    public void DisEmbark()
    {
    }
}