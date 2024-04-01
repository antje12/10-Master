using ClassLibrary.Classes.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(Avatar), typeof(Projectile), typeof(Ship), typeof(Treasure))]
public class Entity
{
    [BsonId] public Guid Id { get; set; }
    [BsonElement("location")] public Coordinates Location { get; set; }
    public TheEntityType Type { get; set; }
    
    public Entity()
    {
        Location = new Coordinates();
    }
    
}

public enum TheEntityType
{
    Player,
    AiAgent,
    Projectile,
    Ship,
    Treasure
}
