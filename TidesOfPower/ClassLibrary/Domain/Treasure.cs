using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Treasure : Entity
{
    [BsonElement("value")] public int Value { get; set; }
    public static readonly int TypeRadius = 24;

    public Treasure(int value, Guid id, Coordinates location) 
        : base(id, location, EntityType.Treasure, TypeRadius)
    {
        Value = value;
    }
    
    public void Collect()
    {
    }
}