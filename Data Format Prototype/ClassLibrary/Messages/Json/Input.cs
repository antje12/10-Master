using ClassLibrary.Messages.Protobuf;
using Newtonsoft.Json;
using Coordinates = ClassLibrary.Classes.Data.Coordinates;

namespace ClassLibrary.Messages.Json;

public class Input
{
    [JsonProperty("agentId")]
    public string AgentId { get; set; }
    [JsonProperty("agentLocation")]
    public Coordinates AgentLocation { get; set; }
    [JsonProperty("mouseLocation")]
    public Coordinates MouseLocation { get; set; }
    [JsonProperty("keyInput")]
    public List<GameKey> KeyInput { get; set; }
    [JsonProperty("gameTime")]
    public double GameTime { get; set; }

    public Input()
    {
        AgentLocation = new Coordinates();
        MouseLocation = new Coordinates();
        KeyInput = new List<GameKey>();
    }
}