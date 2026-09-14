using MonoEngine.Entities;
using MonoLibrary.Colliders;
using MonoLibrary.Colliders.Rectangles;
using Microsoft.Xna.Framework;

namespace MonoEngine.Levels.Objects;
public class HazardObject(Vector2 position) : Interactable(position)
{
    public static HazardObject Build(Vector2 position, Vector2 dimensions, Alignment alignment = Alignment.TopLeft)
    {
        var collider = new HazardObject(position);
        collider.GenerateHitbox((int)dimensions.X, (int)dimensions.Y, alignment);
        return collider;
    }
}