using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Treasure : Entity
{
    [BsonElement("value")] public int Value { get; set; }
    public static readonly int Rad = 24;

    public Treasure(int value, Guid id, Coordinates location) 
        : base(id, location, EntityType.Treasure, Rad)
    {
        Value = value;
    }
    
    public void Collect()
    {
    }
}