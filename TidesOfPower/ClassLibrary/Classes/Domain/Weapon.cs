namespace ClassLibrary.Classes.Domain;

public class Weapon
{
    public string Name { get; set; }
    public Projectile Projectile { get; set; }
    public int Damage { get; set; }
    public int Range { get; set; }
}