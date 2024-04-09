using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Projectile : Entity
{
    [BsonElement("direction")] public Coordinates Direction { get; set; } 
    [BsonElement("time-to-live")] public double TimeToLive { get; set; }
    [BsonElement("damage")] public int Damage { get; set; }
    
    public Projectile()
    {
        Type = EntityType.Projectile;
    }
}