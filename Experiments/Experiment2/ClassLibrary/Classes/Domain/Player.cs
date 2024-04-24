namespace ClassLibrary.Classes.Domain;

public class Player : Avatar
{
    public Profile Profile { get; set; }

    public Player()
    {
        Type = TheEntityType.Player;
    }
}