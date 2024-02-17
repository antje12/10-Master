using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Domain;

public class Ocean
{
    public Direction WindDirection { get; set; }
    public List<Island> Islands { get; set; }
    public List<Ship> Ships { get; set; }
}