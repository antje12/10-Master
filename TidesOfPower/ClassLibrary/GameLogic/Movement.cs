using ClassLibrary.Messages.Protobuf;

namespace ClassLibrary.GameLogic;

public class Movement
{
    private static int avatarSpeed = 100;
    private static int projectileSpeed = 200;

    public static void MoveAvatar(float x, float y, List<GameKey> input, double gameTime, out float toX, out float toY)
    {
        toX = x;
        toY = y;
        foreach (var i in input)
        {
            switch (i)
            {
                case GameKey.Up:
                    toY -= avatarSpeed * (float) gameTime;
                    break;
                case GameKey.Down:
                    toY += avatarSpeed * (float) gameTime;
                    break;
                case GameKey.Left:
                    toX -= avatarSpeed * (float) gameTime;
                    break;
                case GameKey.Right:
                    toX += avatarSpeed * (float) gameTime;
                    break;
            }
        }
    }

    public static void MoveProjectile(float x, float y, float dirX, float dirY, double gameTime, out double time,
        out float toX, out float toY)
    {
        toX = x;
        toY = y;
        time = gameTime * projectileSpeed;

        toX += dirX * projectileSpeed * (float) gameTime;
        toY += dirY * projectileSpeed * (float) gameTime;
    }
}