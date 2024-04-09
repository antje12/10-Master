using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Agent : Entity
{
    [BsonElement("life-pool")] public int LifePool { get; set; }
    [BsonElement("walking-speed")] public int WalkingSpeed { get; set; }
    public Weapon Weapon { get; set; }
    
    public void TakeDamage()
    {
    }
}