using Microsoft.Xna.Framework;

namespace GameClient.Core;

public class Camera
{
    private MyGame _game;
    public Matrix Transform;

    public Camera(MyGame game)
    {
        _game = game;
    }

    public void Follow(Vector2 playerPosition)
    {
        var position = Matrix.CreateTranslation(
            -playerPosition.X,
            -playerPosition.Y,
            0);

        var offset = Matrix.CreateTranslation(
            _game.ScreenWidth / 2,
            _game.ScreenHeight / 2,
            0);

        Transform = position * offset;
    }
    
    public bool MouseOnScreen(Vector2 mouse)
    {
        return 0 <= mouse.X && mouse.X <= _game.ScreenWidth &&
               0 <= mouse.Y && mouse.Y <= _game.ScreenHeight;
    }
    
    public Vector2 MouseInWorld(Vector2 screenPosition)
    {
        Matrix inverseTransform = Matrix.Invert(Transform);
        Vector3 screenPosition3 = new Vector3(screenPosition, 0);
        Vector3 worldPosition = Vector3.Transform(screenPosition3, inverseTransform);
        return new Vector2(worldPosition.X, worldPosition.Y);
    }
}