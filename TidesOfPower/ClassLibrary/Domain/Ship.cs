using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Ship : Entity
{
    [BsonElement("life-pool")] public int LifePool { get; set; }

    public Ship(int lifePool, Guid id, Coordinates location) 
        : base(id, location, EntityType.Ship)
    {
        LifePool = lifePool;
    }
    
    public void Embark()
    {
    }
    
    public void DisEmbark()
    {
    }
}