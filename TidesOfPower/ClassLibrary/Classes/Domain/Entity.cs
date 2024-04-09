using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(Agent), typeof(Projectile), typeof(Ship), typeof(Treasure))]
public class Entity
{
    [BsonId] public Guid Id { get; set; }
    [BsonElement("location")] public Coordinates Location { get; set; }
    public EntityType Type { get; set; }
    
    public Entity()
    {
        Location = new Coordinates();
    }
}

public enum EntityType
{
    Player,
    Enemy,
    Projectile,
    Ship,
    Treasure
}
