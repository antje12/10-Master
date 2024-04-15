using ClassLibrary.Messages.Protobuf;

namespace ClassLibrary.GameLogic;

public static class Move
{
    private static int _agentSpeed { get; set; }
    private static int _projectileSpeed { get; set; }
    
    public static void Agent(
        float x, float y, List<GameKey> input, double gameTime,
        out float toX, out float toY)
    {
        toX = x;
        toY = y;
        foreach (var i in input)
        {
            switch (i)
            {
                case GameKey.Up:
                    toY -= _agentSpeed * (float) gameTime;
                    break;
                case GameKey.Down:
                    toY += _agentSpeed * (float) gameTime;
                    break;
                case GameKey.Left:
                    toX -= _agentSpeed * (float) gameTime;
                    break;
                case GameKey.Right:
                    toX += _agentSpeed * (float) gameTime;
                    break;
            }
        }
    }

    public static void Projectile(
        float x, float y, float dirX, float dirY, double gameTime,
        out double time, out float toX, out float toY)
    {
        toX = x;
        toY = y;
        
        time = gameTime * _projectileSpeed;
        toX += dirX * _projectileSpeed * (float) gameTime;
        toY += dirY * _projectileSpeed * (float) gameTime;
    }
}