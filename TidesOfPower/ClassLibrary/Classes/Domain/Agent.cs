using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Agent : Entity
{
    [BsonElement("life-pool")] public int LifePool { get; set; }
    [BsonElement("walking-speed")] public int WalkingSpeed { get; set; }
    public Weapon Weapon { get; set; }

    public Agent(Guid id, Coordinates location, EntityType type, int lifePool, int walkingSpeed) 
        : base(id, location, type)
    {
        LifePool = lifePool;
        WalkingSpeed = walkingSpeed;
    }
    
    public void TakeDamage(int amount)
    {
        LifePool -= amount;
    }
}