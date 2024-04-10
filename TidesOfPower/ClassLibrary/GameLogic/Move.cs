using ClassLibrary.Messages.Protobuf;

namespace ClassLibrary.GameLogic;

public static class Move
{
    public static void Avatar(
        float x, float y, List<GameKey> input, 
        double gameTime, int speed,
        out float toX, out float toY)
    {
        toX = x;
        toY = y;
        foreach (var i in input)
        {
            switch (i)
            {
                case GameKey.Up:
                    toY -= speed * (float) gameTime;
                    break;
                case GameKey.Down:
                    toY += speed * (float) gameTime;
                    break;
                case GameKey.Left:
                    toX -= speed * (float) gameTime;
                    break;
                case GameKey.Right:
                    toX += speed * (float) gameTime;
                    break;
            }
        }
    }

    public static void Projectile(
        float x, float y, float dirX, float dirY, 
        double gameTime, int speed,
        out double time, out float toX, out float toY)
    {
        toX = x;
        toY = y;
        
        time = gameTime * speed;
        toX += dirX * speed * (float) gameTime;
        toY += dirY * speed * (float) gameTime;
    }
}