using System;
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



public abstract class Collider(Vector2 position)
{
    public Vector2 Position = position;
    public abstract Polygon GenericHitbox { get; }

    public abstract void UpdateHitbox(Vector2 position);
    public abstract void UpdateHitbox(float X, float Y);

    protected Vector2 GetAlignedPosition(float width, float height, Alignment alignment) => GetAlignedPosition(new(width, height), alignment);
    protected Vector2 GetAlignedPosition(Vector2 boundingSize, Alignment alignment)
    {
        float x = Position.X;
        float y = Position.Y;
        float width = boundingSize.X;
        float height = boundingSize.Y;

        return alignment switch
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
}
