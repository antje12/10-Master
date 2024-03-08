using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Domain;

public class Treasure : Entity
{
    public int Value { get; set; }

    public Treasure()
    {
        Type = TheEntityType.Treasure;
    }
}