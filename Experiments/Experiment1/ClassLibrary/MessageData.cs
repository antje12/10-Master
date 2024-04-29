using Newtonsoft.Json;

namespace ClassLibrary;

public class MessageData
{
    [JsonProperty("AgentId")]
    public Guid AgentId {get;set;}
    [JsonProperty("EventId")]
    public Guid EventId {get;set;}
    [JsonProperty("AgentLocation")]
    public Coordinates AgentLocation {get;set;}
    [JsonProperty("MouseLocation")]
    public Coordinates MouseLocation {get;set;}
    [JsonProperty("GameTime")]
    public double GameTime {get;set;}
    
    public MessageData()
    {
        AgentId = Guid.NewGuid();
        EventId = Guid.NewGuid();
        AgentLocation = new Coordinates(10, 10);
        MouseLocation = new Coordinates(10, 10);
        GameTime = 10;
    }
}