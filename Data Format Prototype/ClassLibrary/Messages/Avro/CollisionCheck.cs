using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;

namespace ClassLibrary.Messages.Avro;

public class CollisionCheck : ISpecificRecord
{
    public string EntityId { get; set; }
    public EntityType Entity { get; set; }
    public Coordinates FromLocation { get; set; }
    public Coordinates ToLocation { get; set; }
    public double Timer { get; set; }

    public CollisionCheck()
    {
        FromLocation = new Coordinates();
        ToLocation = new Coordinates();
    }
    
    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Messages"",
        ""type"": ""record"",
        ""name"": ""CollisionCheck"",
        ""fields"": [
            {{ ""name"": ""EntityId"", ""type"": ""string"" }},
            {{
                ""name"": ""Entity"", 
                ""type"": {{
                    ""type"": ""enum"",
                    ""name"": ""EntityType"",
                    ""symbols"": [""Player"", ""Bullet"", ""Ai""]
                }}
            }},
            {{ ""name"": ""FromLocation"", ""type"": {Coordinates.StatSchema()} }},
            {{ ""name"": ""ToLocation"", ""type"": ""ClassLibrary.Classes.Data.Coordinates"" }},
            {{ ""name"": ""Timer"", ""type"": ""double"" }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return EntityId;
            case 1: return Entity;
            case 2: return FromLocation;
            case 3: return ToLocation;
            case 4: return Timer;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                EntityId = (string) fieldValue;
                break;
            case 1:
                Entity = (EntityType) fieldValue;
                break;
            case 2:
                FromLocation = (Coordinates) fieldValue;
                break;
            case 3:
                ToLocation = (Coordinates) fieldValue;
                break;
            case 4:
                Timer = (double) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}