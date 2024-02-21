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
}