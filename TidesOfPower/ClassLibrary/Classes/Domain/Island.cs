using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Domain;

public class Island
{
    public List<Coordinates> NorthShoreline { get; set; }
    public List<Coordinates> SouthShoreline { get; set; }
    public List<Coordinates> EastShoreline { get; set; }
    public List<Coordinates> WestShoreline { get; set; }
    public List<Avatar> Avatars { get; set; }
    public List<Treasure> Treasures { get; set; }
}