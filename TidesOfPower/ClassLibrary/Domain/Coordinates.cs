﻿using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Coordinates
{
    [BsonElement("x")] public float X { get; set; }
    [BsonElement("y")] public float Y { get; set; }

    public Coordinates(float x, float y)
    {
        X = x;
        Y = y;
    }
}