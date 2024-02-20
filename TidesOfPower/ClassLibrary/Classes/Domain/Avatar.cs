using ClassLibrary.Classes.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Avatar
{
    public Avatar()
    {
        Location = new Coordinates();
    }
    
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }
    [BsonElement("location")]
    public Coordinates Location { get; set; }
    [BsonElement("walking-speed")]
    public int WalkingSpeed { get; set; }
    [BsonElement("life-pool")]
    public int LifePool { get; set; }
    [BsonElement("inventory")]
    public int Inventory { get; set; }
    public List<Weapon> Weapons { get; set; }
    public Ship Ship { get; set; }
}