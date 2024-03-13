using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameClient.Core;

public class Camera
{
    public Matrix Transform { get; private set; }

    public void Follow(Vector2 playerPosition, Texture2D playerTexture, int screenWidth, int screenHeight)
    {
        var position = Matrix.CreateTranslation(
            -playerPosition.X, //- (playerTexture.Width / 2),
            -playerPosition.Y, //- (playerTexture.Height / 2),
            0);

        var offset = Matrix.CreateTranslation(
            screenWidth / 2,
            screenHeight / 2,
            0);

        Transform = position * offset;
    }
    
    public bool MouseOnScreen(Vector2 mouse)
    {
        return 0 <= mouse.X && mouse.X <= MyGame.screenWidth &&
               0 <= mouse.Y && mouse.Y <= MyGame.screenHeight;
    }
    
    public Vector2 MouseInWorld(Vector2 screenPosition)
    {
        Matrix inverseTransform = Matrix.Invert(Transform);
        Vector3 screenPosition3 = new Vector3(screenPosition, 0);
        Vector3 worldPosition = Vector3.Transform(screenPosition3, inverseTransform);
        return new Vector2(worldPosition.X, worldPosition.Y);
    }
}