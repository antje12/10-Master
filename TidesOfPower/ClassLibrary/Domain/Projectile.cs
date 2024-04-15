using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Projectile : Entity
{
    [BsonElement("direction")] public Coordinates Direction { get; set; } 
    [BsonElement("flying-speed")] public int FlyingSpeed { get; set; }
    [BsonElement("time-to-live")] public double TimeToLive { get; set; }
    [BsonElement("damage")] public int Damage { get; set; }
    public static readonly int TypeRadius = 5;
    
    public Projectile(Coordinates direction, int flyingSpeed, double timeToLive, int damage, Guid id, Coordinates location) 
        : base(id, location, EntityType.Projectile, TypeRadius)
    {
        Direction = direction;
        FlyingSpeed = flyingSpeed;
        TimeToLive = timeToLive;
        Damage = damage;
    }
}