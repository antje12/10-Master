using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;

namespace ClassLibrary.Messages.Avro;

public class Input : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public Coordinates PlayerLocation { get; set; }
    public Coordinates MouseLocation { get; set; }
    public List<GameKey> KeyInput { get; set; }
    public double GameTime { get; set; }

    public Input()
    {
        PlayerLocation = new Coordinates();
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
            {{ ""name"": ""PlayerId"", ""type"": ""string"" }},
            {{ ""name"": ""PlayerLocation"", ""type"": {Coordinates.StatSchema()} }},
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
            {{ ""name"": ""Timer"", ""type"": ""double"" }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return PlayerId.ToString();
            case 1: return PlayerLocation;
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
                PlayerId = Guid.Parse((string) fieldValue);
                break;
            case 1:
                PlayerLocation = (Coordinates) fieldValue;
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