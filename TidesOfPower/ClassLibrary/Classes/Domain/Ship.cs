using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Domain;

public class Ship
{
    public string Name { get; set; }
    public Coordinates Location { get; set; }
    public int LifePool { get; set; }
}