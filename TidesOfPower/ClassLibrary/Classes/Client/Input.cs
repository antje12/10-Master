using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Client
{
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
                    ""name"": ""X"",
                    ""type"": ""float""
                },
                {
                    ""name"": ""Y"",
                    ""type"": ""float""
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
                case 1: return Location.X;
                case 2: return Location.Y;
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
                    PlayerId = Guid.Parse((string)fieldValue);
                    break;
                case 1:
                    Location.X = (float)fieldValue;
                    break;
                case 2:
                    Location.Y = (float)fieldValue;
                    break;
                case 3:
                    KeyInput = (List<GameKey>)fieldValue;
                    break;
                case 4:
                    GameTime = (double)fieldValue;
                    break;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
            }
        }
    }
}
