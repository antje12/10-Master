using ClassLibrary.Domain;
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

    public void Follow(Coordinates playerLocation)
    {
        var location = Matrix.CreateTranslation(
            -playerLocation.X,
            -playerLocation.Y,
            0);

        var offset = Matrix.CreateTranslation(
            _game.ScreenWidth / 2,
            _game.ScreenHeight / 2,
            0);

        Transform = location * offset;
    }
    
    public bool MouseOnScreen(Vector2 mouse)
    {
        return 0 <= mouse.X && mouse.X <= _game.ScreenWidth &&
               0 <= mouse.Y && mouse.Y <= _game.ScreenHeight;
    }
    
    public Vector2 MouseInWorld(Vector2 screenLocation)
    {
        Matrix inverseTransform = Matrix.Invert(Transform);
        Vector3 screenLocation3 = new Vector3(screenLocation, 0);
        Vector3 worldLocation = Vector3.Transform(screenLocation3, inverseTransform);
        return new Vector2(worldLocation.X, worldLocation.Y);
    }
}