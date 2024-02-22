using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;

namespace ClassLibrary.Classes.Messages;

public class LocalState : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public Coordinates Location { get; set; }
    public List<Avatar> Avatars { get; set; }

    public LocalState()
    {
        Location = new Coordinates();
        Avatars = new List<Avatar>();
    }
    
    public Schema Schema => Schema.Parse(@"
    {
        ""namespace"": ""ClassLibrary.Classes.Messages"",
        ""type"": ""record"",
        ""name"": ""LocalState"",
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
            },
            {
                ""name"": ""Avatars"",
                ""type"": {
                    ""type"": ""array"",
                    ""items"": {
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
                    }
                }
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
            case 3: return Avatars;
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
                Location.X = (float) fieldValue;
                break;
            case 2:
                Location.Y = (float) fieldValue;
                break;
            case 3:
                Avatars = (List<Avatar>) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}