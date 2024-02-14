using Avro;
using Avro.Specific;

namespace ClassLibrary.Classes.Client;

public class Input : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public Coordinates Location { get; set; }
    public Direction DirectionalInput { get; set; }
    public double Timer { get; set; }

    public Input()
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
            },
            {
                ""name"": ""DirectionalInput"",
                ""type"": {
                    ""type"": ""enum"",
                    ""name"": ""Direction"",
                    ""symbols"": [
                        ""Stay"",
                        ""North"",
                        ""South"",
                        ""East"",
                        ""West"",
                        ""NorthEast"",
                        ""SouthEast"",
                        ""SouthWest"",
                        ""NorthWest""
                    ]
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
            case 1: return Location.X;
            case 2: return Location.Y;
            case 3: return DirectionalInput;
            case 4: return Timer;
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
                DirectionalInput = (Direction) fieldValue;
                break;
            case 4:
                Timer = (double) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}