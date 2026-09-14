using MonoLibrary.Colliders.Rectangles;
using Microsoft.Xna.Framework;

namespace MonoEngine.Levels.Objects;
public class SolidObject(Vector2 position) : Interactable(position)
{
    public static SolidObject Build(Vector2 position, Vector2 dimensions, Alignment alignment = Alignment.TopLeft)
    {
        var collider = new SolidObject(position);
        collider.GenerateHitbox((int)dimensions.X, (int)dimensions.Y, alignment);
        return collider;
    }
}