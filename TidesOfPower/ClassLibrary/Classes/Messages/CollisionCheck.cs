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

    public Schema Schema => Schema.Parse(@"
    {
        ""namespace"": ""git.avro"",
        ""type"": ""record"",
        ""name"": ""CollisionCheck"",
        ""fields"": [
            {
                ""name"": ""PlayerId"",
                ""type"": ""string""
            },
            {
                ""name"": ""FromLocation"",
                ""type"": {
                      ""type"": ""record"",
                      ""name"": ""Coordinates"",
                      ""fields"": [
                      { ""name"": ""X"", ""type"": ""float"" },
                      { ""name"": ""Y"", ""type"": ""float"" }
                      ]
                }
            },
            {
                ""name"": ""ToLocation"",
                ""type"": ""git.avro.Coordinates""
            }
        ]
    }");

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