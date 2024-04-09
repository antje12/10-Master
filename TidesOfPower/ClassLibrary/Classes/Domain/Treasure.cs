using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Treasure : Entity
{
    [BsonElement("value")] public int Value { get; set; }

    public Treasure(int value, Guid id, Coordinates location) 
        : base(id, location, EntityType.Treasure)
    {
        Value = value;
    }
    
    public void Collect()
    {
    }
}