using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Coordinates
{
    [BsonElement("x")] public float X { get; set; }
    [BsonElement("y")] public float Y { get; set; }
}