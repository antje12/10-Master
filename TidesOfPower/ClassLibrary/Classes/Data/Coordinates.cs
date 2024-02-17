using Avro;
using Avro.Specific;

namespace ClassLibrary.Classes.Data;

public class Coordinates : ISpecificRecord
{
    public float X { get; set; }
    public float Y { get; set; }

    public Schema Schema => Schema.Parse(@"
        {
            ""namespace"": ""git.avro"",
            ""type"": ""record"",
            ""name"": ""Coordinates"",
            ""fields"": [
                { ""name"": ""X"", ""type"": ""float"" },
                { ""name"": ""Y"", ""type"": ""float"" }
            ]
        }");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return X;
            case 1: return Y;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                X = (float) fieldValue;
                break;
            case 1:
                Y = (float) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}
