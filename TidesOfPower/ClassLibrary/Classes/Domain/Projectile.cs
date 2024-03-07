using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Projectile : Entity, ISpecificRecord
{
    [BsonElement("direction")] public Coordinates Direction { get; set; } 
    [BsonElement("timer")] public double Timer { get; set; }

    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Domain"",
        ""type"": ""record"",
        ""name"": ""Projectile"",
        ""fields"": [
            {{ ""name"": ""Id"", ""type"": ""string"" }},
            {{ ""name"": ""Location"", ""type"": {Coordinates.StatSchema("Projectile")} }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return Id.ToString();
            case 1: return Location;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                Id = Guid.Parse((string) fieldValue);
                break;
            case 1:
                Location = (Coordinates) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}