using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Messages;

public class Avatar : ISpecificRecord
{
    public Avatar()
    {
        Location = new Coordinates();
    }
    
    public Schema Schema => Schema.Parse(@"
        {
            ""namespace"": ""ClassLibrary.Classes.Messages"",
            ""type"": ""record"",
            ""name"": ""Avatar"",
            ""fields"": [
                { ""name"": ""Id"", ""type"": ""string"" },
                { ""name"": ""Name"", ""type"": ""string"" },
                {
                    ""name"": ""Location"",
                    ""type"": {
                        ""namespace"": ""ClassLibrary.Classes.Messages"",
                        ""type"": ""record"",
                        ""name"": ""Coordinates"",
                        ""fields"": [
                            { ""name"": ""X"", ""type"": ""float"" },
                            { ""name"": ""Y"", ""type"": ""float"" }
                        ]
                    }
                }
            ]
        }");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return Id.ToString();
            case 1: return Name;
            case 2: return Location;
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
                Name = (string) fieldValue;
                break;
            case 2:
                Location = (Coordinates) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
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