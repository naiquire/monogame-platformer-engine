using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoLibrary.Structures;

namespace MonoLibrary.Colliders;
public enum Alignment
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Center,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}
public struct HitboxManager<T>(Alignment alignment) where T : Polygon
{
    public T Hitbox { get; private set; }
    public T PreviousHitbox { get; private set; }
    public Alignment Alignment { get; } = alignment;

    public void LoadHitbox(T hitbox)
    {
        PreviousHitbox = Hitbox;
        Hitbox = hitbox;
    }
}    

public abstract class Collider<T>(Vector2 position) where T : Polygon
{
    public Vector2 Position = position;
    protected HitboxManager<T> HitboxManager;

    public T Hitbox => HitboxManager.Hitbox;
    public T PreviousHitbox => HitboxManager.PreviousHitbox;
    public Alignment HitboxAlignment => HitboxManager.Alignment;

    protected Vector2 GetAlignedPosition(float width, float height) => GetAlignedPosition(new(width, height));
    protected Vector2 GetAlignedPosition(Vector2 boundingSize)
    {
        float x = Position.X;
        float y = Position.Y;
        float width = boundingSize.X;
        float height = boundingSize.Y;

        return HitboxManager.Alignment switch
        {
            Alignment.TopLeft => new(x, y),
            Alignment.Top => new(x, y),
            Alignment.TopRight => new(x - width, y),
            Alignment.Left => new(x, y - height / 2),
            Alignment.Center => new(x - width / 2, y - height / 2),
            Alignment.Right => new(x - width, y - height / 2),
            Alignment.BottomLeft => new(x, y - height),
            Alignment.Bottom => new(x - width / 2, y - height),
            Alignment.BottomRight => new(x - width, y - height),

            _ => throw new InvalidEnumArgumentException()
        };
    }

    public abstract bool SweptAABB(T currentHitbox, T newHitbox, Polygon obj);

    public abstract void UpdateHitbox(Vector2 position);
    public abstract void UpdateHitbox(float X, float Y);
}
