using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Domain;

public class Player : Agent
{
    [BsonElement("name")] public string Name { get; set; }
    [BsonElement("score")] public int Score { get; set; }

    public Profile Profile { get; set; }

    public Player(string name, int score, Guid id, Coordinates location, int lifePool, int walkingSpeed) 
        : base(id, location, EntityType.Player, lifePool, walkingSpeed)
    {
        Name = name;
        Score = score;
    }
}