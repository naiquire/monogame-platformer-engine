using System;
using Microsoft.Xna.Framework;
using MonoLibrary.Structures;

namespace MonoLibrary.Colliders;
public sealed class CircleCollider(Vector2 position) : Collider<Circle>(position)
{
    public void GenerateHitbox(float radius, Alignment alignment = Alignment.TopLeft)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(radius);

        HitboxManager = new HitboxManager<Circle>(alignment);
        Vector2 alignedPosition = GetAlignedPosition(new(2 * radius));

        Circle hitbox = new(alignedPosition, radius);
        HitboxManager.LoadHitbox(hitbox);
    }

    public override void UpdateHitbox(Vector2 position)
    {
        Position = position;
        Vector2 alignedPosition = GetAlignedPosition(Hitbox.Diameter, Hitbox.Diameter);
        
        Circle hitbox = new(alignedPosition, Hitbox.Radius);
        HitboxManager.LoadHitbox(hitbox);
    }
    public override void UpdateHitbox(float X, float Y) => UpdateHitbox(new(X, Y));

    public override bool SweptAABB(Circle currentHitbox, Circle newHitbox, Polygon obj)
    {
        if (obj.Intersects(currentHitbox) || obj.Intersects(newHitbox)) return true;

        // vertically aligned
        if (currentHitbox.X == newHitbox.X)
        {
            var fill = new Quadrilateral(currentHitbox.Left, Math.Min(currentHitbox.Center.Y, newHitbox.Center.Y),
                currentHitbox.Diameter, Math.Abs(currentHitbox.Center.Y - newHitbox.Center.Y));
            return obj.Intersects(fill);
        }

        // horizontally aligned
        else if (currentHitbox.Y == newHitbox.Y)
        {
            var fill = new Quadrilateral(Math.Min(currentHitbox.Center.X, newHitbox.Center.X), currentHitbox.Top,
                Math.Abs(currentHitbox.Center.X - newHitbox.Center.X), currentHitbox.Diameter);
            return obj.Intersects(fill);
        }

        else
        {
            throw new ArgumentException("Swept AABB can only be calculated during motions in one dimension");
        }
    }
}