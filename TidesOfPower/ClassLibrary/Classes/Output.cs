using Avro;
using Avro.Specific;

namespace ClassLibrary.Classes;

public class Output : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public Coordinates Location { get; set; }
    
    public Output()
    {
        Location = new Coordinates();
    }
    
    public Schema Schema => Schema.Parse(@"
    {
        ""namespace"": ""git.avro"",
        ""type"": ""record"",
        ""name"": ""Input"",
        ""fields"": [
            {
                ""name"": ""PlayerId"",
                ""type"": ""string""
            },
            {
                ""name"": ""X"",
                ""type"": ""float""
            },
            {
                ""name"": ""Y"",
                ""type"": ""float""
            }
        ]
    }");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return PlayerId.ToString();
            case 1: return Location.X;
            case 2: return Location.Y;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: PlayerId = Guid.Parse((string)fieldValue); break;
            case 1: Location.X = (float)fieldValue; break;
            case 2: Location.Y = (float)fieldValue; break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}