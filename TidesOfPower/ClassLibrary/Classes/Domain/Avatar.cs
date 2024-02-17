using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Domain;

public class Avatar
{
    public string Name { get; set; }
    public Coordinates Location { get; set; }
    public int WalkingSpeed { get; set; }
    public int LifePool { get; set; }
    public int Inventory { get; set; }
    public List<Weapon> Weapons { get; set; }
    public Ship Ship { get; set; }
}