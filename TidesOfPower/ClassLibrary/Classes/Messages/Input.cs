using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Messages;

public class Input : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public Coordinates Location { get; set; }
    public List<GameKey> KeyInput { get; set; }
    public double GameTime { get; set; }

    public Input()
    {
        Location = new Coordinates();
        KeyInput = new List<GameKey>();
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
                    ""name"": ""Location"",
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
                    ""name"": ""KeyInput"",
                    ""type"": {
                        ""type"": ""array"",
                        ""items"": {
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
                        }
                    }
                },
                {
                    ""name"": ""Timer"",
                    ""type"": ""double""
                }
            ]
        }");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return PlayerId.ToString();
            case 1: return Location;
            case 2: return KeyInput;
            case 3: return GameTime;
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
                Location = (Coordinates) fieldValue;
                break;
            case 2:
                KeyInput = (List<GameKey>) fieldValue;
                break;
            case 3:
                GameTime = (double) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}
