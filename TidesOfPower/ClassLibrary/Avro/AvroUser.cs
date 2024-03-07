using Avro;
using Avro.Specific;

namespace ClassLibrary.Avro;

public class AvroUser : ISpecificRecord
{
    public string Name { get; set; }
    public int FavoriteNumber { get; set; }
    public string FavoriteColor { get; set; }
    
    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Messages"",
        ""type"": ""record"",
        ""name"": ""AvroUser"",
        ""fields"": [
            {{ ""name"": ""Name"", ""type"": ""string"" }},
            {{ ""name"": ""FavoriteNumber"", ""type"": ""int"" }},
            {{ ""name"": ""FavoriteColor"", ""type"": ""string"" }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return Name;
            case 1: return FavoriteNumber;
            case 2: return FavoriteColor;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                Name = (string) fieldValue;
                break;
            case 1:
                FavoriteNumber = (int) fieldValue;
                break;
            case 2:
                FavoriteColor = (string) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}