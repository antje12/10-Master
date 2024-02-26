using ClassLibrary.Classes.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(Avatar), typeof(Projectile), typeof(Ship), typeof(Treasure))]
public class Entity
{
    [BsonId] public Guid Id { get; set; }
    [BsonElement("location")] public Coordinates Location { get; set; }
    
    public Entity()
    {
        Location = new Coordinates();
    }
    
}