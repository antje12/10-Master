using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Island
{
    [BsonElement("from-x")] public int fromX { get; set; }
    [BsonElement("to-x")] public int toX { get; set; }
    [BsonElement("from-y")] public int fromY { get; set; }
    [BsonElement("to-y")] public int toY { get; set; }
    public List<Entity> Entities { get; set; }
}