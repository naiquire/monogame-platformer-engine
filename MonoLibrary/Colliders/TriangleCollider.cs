using System;
using Microsoft.Xna.Framework;
using MonoLibrary.Structures;

namespace MonoLibrary.Colliders;
public sealed class TriangleCollider(Vector2 position) : Collider(position)
{
    private HitboxManager<RightTriangle> HitboxManager;

    public RightTriangle Hitbox => HitboxManager.Hitbox;
    public RightTriangle PreviousHitbox => HitboxManager.PreviousHitbox;
    public Alignment HitboxAlignment => HitboxManager.Alignment;
    public override Polygon GenericHitbox => Hitbox;

    public void GenerateHitbox(Vector2 dimensions, RightTriangle.NormalDirection direction, Alignment alignment = Alignment.TopLeft) => GenerateHitbox(dimensions.X, dimensions.Y, direction, alignment);
    public void GenerateHitbox(float width, float height, RightTriangle.NormalDirection direction, Alignment alignment = Alignment.TopLeft)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        HitboxManager = new HitboxManager<RightTriangle>(alignment);
        Vector2 alignedPosition = GetAlignedPosition(width, height, alignment);

        RightTriangle hitbox = new(alignedPosition.X, alignedPosition.Y, width, height, direction);
        HitboxManager.LoadHitbox(hitbox);
    }

    public override void UpdateHitbox(Vector2 position)
    {
        Position = position;
        Vector2 alignedPosition = GetAlignedPosition(Hitbox.BoundingSize, HitboxAlignment);
        
        RightTriangle hitbox = new(alignedPosition, Hitbox.BoundingSize, Hitbox.Direction);
        HitboxManager.LoadHitbox(hitbox);
    }
    public override void UpdateHitbox(float X, float Y) => UpdateHitbox(new(X, Y));

    public bool SweptAABB(RightTriangle currentHitbox, RightTriangle newHitbox, Polygon obj)
    {
        throw new NotImplementedException();
    }
}