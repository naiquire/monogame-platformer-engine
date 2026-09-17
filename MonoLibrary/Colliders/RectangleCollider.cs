using System;
using MonoLibrary.Structures;

namespace MonoLibrary.Colliders;
public sealed class RectangleCollider(Vector2 position) : Collider<Quadrilateral>(position)
{
    public void GenerateHitbox(Vector2 dimensions, Alignment alignment = Alignment.TopLeft) => GenerateHitbox(dimensions.X, dimensions.Y, alignment);
    public void GenerateHitbox(float width, float height, Alignment alignment = Alignment.TopLeft)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("The width and height of the hitbox must be greater than zero.");
        }

        HitboxManager = new HitboxManager<Quadrilateral>(alignment);
        Vector2 alignedPosition = GetAlignedPosition(width, height);

        Quadrilateral hitbox = new(alignedPosition.X, alignedPosition.Y, width, height);
        HitboxManager.LoadHitbox(hitbox);
    }

    public override void UpdateHitbox(Vector2 position)
    {
        Position = position;
        Vector2 alignedPosition = GetAlignedPosition(Hitbox.BoundingSize);

        Quadrilateral hitbox = new(alignedPosition, Hitbox.BoundingSize);
        HitboxManager.LoadHitbox(hitbox);
    }
    public override void UpdateHitbox(float X, float Y) => UpdateHitbox(new(X, Y));

    public override bool SweptAABB(Quadrilateral currentHitbox, Quadrilateral newHitbox, Polygon obj)
    {
        Quadrilateral union = Quadrilateral.Union(currentHitbox, newHitbox);
        return obj.Intersects(union);
    }
}