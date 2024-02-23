using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Avatar : ISpecificRecord
{
    public Avatar()
    {
        Location = new Coordinates();
    }

    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Domain"",
        ""type"": ""record"",
        ""name"": ""Avatar"",
        ""fields"": [
            {{ ""name"": ""Id"", ""type"": ""string"" }},
            {{ ""name"": ""Location"", ""type"": {Coordinates.StatSchema} }}
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

    [BsonId] public Guid Id { get; set; }
    [BsonElement("location")] public Coordinates Location { get; set; }

    [BsonElement("name")] public string Name { get; set; }

    [BsonElement("walking-speed")] public int WalkingSpeed { get; set; }
    [BsonElement("life-pool")] public int LifePool { get; set; }
    [BsonElement("inventory")] public int Inventory { get; set; }
    public List<Weapon> Weapons { get; set; }
    public Ship Ship { get; set; }
}