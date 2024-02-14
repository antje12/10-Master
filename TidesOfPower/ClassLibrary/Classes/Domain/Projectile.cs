namespace ClassLibrary.Classes.Domain;

public class Projectile
{
    public Coordinates Location { get; set; }
    public Direction Direction { get; set; }
    public int Timer { get; set; }
}