using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Weapon
{
    [BsonElement("name")] public string Name { get; set; }
    [BsonElement("range")] public int Range { get; set; }
    [BsonElement("damage")] public int Damage { get; set; }
    
    public void Shoot()
    {
    }
}