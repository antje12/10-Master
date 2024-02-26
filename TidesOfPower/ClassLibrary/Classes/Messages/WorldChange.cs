using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Messages;

public class WorldChange : ISpecificRecord
{
    public Guid EntityId { get; set; }
    public ChangeType Change { get; set; }
    public Coordinates Location { get; set; }
    public Coordinates Direction { get; set; }

    public WorldChange()
    {
        Location = new Coordinates();
        Direction = new Coordinates();
    }

    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Messages"",
        ""type"": ""record"",
        ""name"": ""WorldChange"",
        ""fields"": [
            {{ ""name"": ""EntityId"", ""type"": ""string"" }},
            {{
                ""name"": ""Change"", 
                ""type"": {{
                    ""type"": ""enum"",
                    ""name"": ""ChangeType"",
                    ""symbols"": [""MovePlayer"", ""SpawnBullet"", ""MoveBullet""]
                }}
            }},
            {{ ""name"": ""Location"", ""type"": {Coordinates.StatSchema} }},
            {{ ""name"": ""Direction"", ""type"": ""ClassLibrary.Classes.Data.Coordinates"" }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return EntityId.ToString();
            case 1: return Change;
            case 2: return Location;
            case 3: return Direction;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                EntityId = Guid.Parse((string) fieldValue);
                break;
            case 1:
                Change = (ChangeType) fieldValue;
                break;
            case 2:
                Location = (Coordinates) fieldValue;
                break;
            case 3:
                Direction = (Coordinates) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}