using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Island
{
    [BsonElement("from-x")] public int FromX { get; set; }
    [BsonElement("to-x")] public int ToX { get; set; }
    [BsonElement("from-y")] public int FromY { get; set; }
    [BsonElement("to-y")] public int ToY { get; set; }
    public List<Entity> Entities { get; set; }

    public Island(int fromX, int toX, int fromY, int toY)
    {
        FromX = fromX;
        ToX = toX;
        FromY = fromY;
        ToY = toY;
        Entities = new List<Entity>();
    }
}