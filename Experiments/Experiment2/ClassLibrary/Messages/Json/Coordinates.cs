using Avro;
using Avro.Specific;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ClassLibrary.Messages.Json;

public class Coordinates
{
    [JsonProperty("x")]
    [BsonElement("x")] 
    public float X { get; set; }
    [JsonProperty("y")]
    [BsonElement("y")] 
    public float Y { get; set; }
}