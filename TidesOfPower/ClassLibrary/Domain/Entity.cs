using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(Agent), typeof(Projectile), typeof(Ship), typeof(Treasure))]
public class Entity
{
    [BsonId] public Guid Id { get; set; }
    [BsonElement("location")] public Coordinates Location { get; set; }
    public EntityType Type { get; set; }
    public int Radius { get; set; }
    
    public Entity(Guid id, Coordinates location, EntityType type, int radius)
    {
        Id = id;
        Location = location;
        Type = type;
        Radius = radius;
    }
}

public enum EntityType
{
    Projectile,
    Treasure,
    Player,
    Enemy,
    Ship
}
