using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Treasure : Entity
{
    [BsonElement("value")] public int Value { get; set; }

    public Treasure()
    {
        Type = EntityType.Treasure;
    }
    
    public void Collect()
    {
    }
}