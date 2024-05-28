using Avro;
using Avro.Specific;
using ClassLibrary.Messages.Protobuf;
using Coordinates = ClassLibrary.Classes.Data.Coordinates;

namespace ClassLibrary.Messages.Avro;

public class Input : ISpecificRecord
{
    public String AgentId { get; set; }
    public Coordinates AgentLocation { get; set; }
    public Coordinates MouseLocation { get; set; }
    public List<GameKey> KeyInput { get; set; }
    public double GameTime { get; set; }

    public Input()
    {
        AgentLocation = new Coordinates();
        MouseLocation = new Coordinates();
        KeyInput = new List<GameKey>();
    }

    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Messages"",
        ""type"": ""record"",
        ""name"": ""Input"",
        ""fields"": [
            {{ ""name"": ""AgentId"", ""type"": ""string"" }},
            {{ ""name"": ""AgentLocation"", ""type"": {Coordinates.StatSchema()} }},
            {{ ""name"": ""MouseLocation"", ""type"": ""ClassLibrary.Classes.Data.Coordinates"" }},
            {{
                ""name"": ""KeyInput"",
                ""type"": {{
                    ""type"": ""array"",
                    ""items"": {{
                        ""type"": ""enum"",
                        ""name"": ""GameKey"",
                        ""symbols"": [
                            ""Up"",
                            ""Down"",
                            ""Left"",
                            ""Right"",
                            ""Attack"",
                            ""Interact""
                        ]
                    }}
                }}
            }},
            {{ ""name"": ""GameTime"", ""type"": ""double"" }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return AgentId;
            case 1: return AgentLocation;
            case 2: return MouseLocation;
            case 3: return KeyInput;
            case 4: return GameTime;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                AgentId = (string) fieldValue;
                break;
            case 1:
                AgentLocation = (Coordinates) fieldValue;
                break;
            case 2:
                MouseLocation = (Coordinates) fieldValue;
                break;
            case 3:
                KeyInput = (List<GameKey>) fieldValue;
                break;
            case 4:
                GameTime = (double) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}