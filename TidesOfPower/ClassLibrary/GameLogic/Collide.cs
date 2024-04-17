namespace ClassLibrary.GameLogic;

public static class Collide
{
    public static bool Circle(
        float e1X, float e1Y, int e1R, 
        float e2X, float e2Y, int e2R)
    {
        float dx = e1X - e2X;
        float dy = e1Y - e2Y;
        // a^2 + b^2 = c^2
        // c = sqrt(a^2 + b^2)
        double distance = Math.Sqrt(dx * dx + dy * dy);
        // if radius overlap
        if (distance < e1R + e2R)
        {
            // Collision!
            return true;
        }
        return false;
    }
}