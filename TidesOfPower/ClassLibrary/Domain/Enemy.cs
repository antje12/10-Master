namespace ClassLibrary.Domain;

public class Enemy : Agent
{
    public Enemy(Guid id, Coordinates location, int lifePool, int walkingSpeed) 
        : base(id, location, EntityType.Enemy, lifePool, walkingSpeed)
    {
    }
    
    public void TakePolicyAction()
    {
    }
}