using Newtonsoft.Json;

namespace ClassLibrary;

public class Coordinates
{
    [JsonProperty("X")]
    public float X { get; set; }
    [JsonProperty("Y")]
    public float Y { get; set; }

    public Coordinates(float x, float y)
    {
        X = x;
        Y = y;
    }
}