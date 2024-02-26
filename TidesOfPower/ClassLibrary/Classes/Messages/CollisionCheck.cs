using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Messages;

public class CollisionCheck : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public Coordinates FromLocation { get; set; }
    public Coordinates ToLocation { get; set; }

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
            {{ ""name"": ""PlayerId"", ""type"": ""string"" }},
            {{ ""name"": ""FromLocation"", ""type"": {Coordinates.StatSchema()} }},
            {{ ""name"": ""ToLocation"", ""type"": ""ClassLibrary.Classes.Data.Coordinates"" }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return PlayerId.ToString();
            case 1: return FromLocation;
            case 2: return ToLocation;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                PlayerId = Guid.Parse((string) fieldValue);
                break;
            case 1:
                FromLocation = (Coordinates) fieldValue;
                break;
            case 2:
                ToLocation = (Coordinates) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}