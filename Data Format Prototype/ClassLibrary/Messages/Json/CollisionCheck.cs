using ClassLibrary.Classes.Data;
using Newtonsoft.Json;

namespace ClassLibrary.Messages.Json;

public class CollisionCheck
{
    [JsonProperty("entityId")]
    public string EntityId { get; set; }
    [JsonProperty("entityType")]
    public EntityType EntityType { get; set; }
    [JsonProperty("fromLocation")]
    public Coordinates FromLocation { get; set; }
    [JsonProperty("toLocation")]
    public Coordinates ToLocation { get; set; }
    [JsonProperty("timer")]
    public double Timer { get; set; }

    public CollisionCheck()
    {
        FromLocation = new Coordinates();
        ToLocation = new Coordinates();
    }
}