namespace ClassLibrary.Classes.Domain;

public class Ocean
{
    public List<Entity> Entities { get; set; }
    public List<Island> Islands { get; set; }

    public Ocean()
    {
        Entities = new List<Entity>();
        Islands = new List<Island>();
    }
}