using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Domain;

public class Ship : Entity
{
    public string Name { get; set; }
    public int LifePool { get; set; }
}