using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Player : Agent
{
    [BsonElement("name")] public string Name { get; set; }
    [BsonElement("score")] public int Score { get; set; }

    public Profile Profile { get; set; }

    public Player()
    {
        Type = EntityType.Player;
    }
}