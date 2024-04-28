using ClassLibrary.Messages.Protobuf;

namespace ClassLibrary.GameLogic;

public static class Move
{
    private static int _avatarSpeed = 100;
    private static int _projectileSpeed = 200;

    public static void Avatar(
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
                    toY -= _avatarSpeed * (float) gameTime;
                    break;
                case GameKey.Down:
                    toY += _avatarSpeed * (float) gameTime;
                    break;
                case GameKey.Left:
                    toX -= _avatarSpeed * (float) gameTime;
                    break;
                case GameKey.Right:
                    toX += _avatarSpeed * (float) gameTime;
                    break;
            }
        }
    }
}