using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Projectile : Entity
{
    [BsonElement("direction")] public Coordinates Direction { get; set; } 
    [BsonElement("time-to-live")] public double TimeToLive { get; set; }
    [BsonElement("damage")] public int Damage { get; set; }
    
    public Projectile(Coordinates direction, double timeToLive, int damage, Guid id, Coordinates location) 
        : base(id, location, EntityType.Projectile)
    {
        Direction = direction;
        TimeToLive = timeToLive;
        Damage = damage;
    }
}