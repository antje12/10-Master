using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Domain;

namespace ClassLibrary.Classes.Messages;

public class LocalState : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public List<Avatar> Avatars { get; set; }

    public LocalState()
    {
        Avatars = new List<Avatar>();
    }

    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Messages"",
        ""type"": ""record"",
        ""name"": ""LocalState"",
        ""fields"": [
            {{ ""name"": ""PlayerId"", ""type"": ""string"" }},
            {{ 
                ""name"": ""Avatars"", 
                ""type"": {{ 
                    ""type"": ""array"", 
                    ""items"": {Avatar.StatSchema} 
                }} 
            }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return PlayerId.ToString();
            case 1: return Avatars;
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
                Avatars = (List<Avatar>) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}