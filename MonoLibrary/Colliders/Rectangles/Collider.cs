using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace MonoLibrary.Colliders.Rectangles;

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

public class Collider(Vector2 position)
{
    /// <summary>
    /// Defines a rectangular region from which collisions are calculated with.
    /// </summary>
    protected HitboxManager Hitbox;
    public Rectangle GetHitbox() => Hitbox.Hitbox;
    public Rectangle GetPreviousHitbox() => Hitbox.PreviousHitbox;

    /// <summary>
    /// The position of the <see cref="Collider"/> with respect to the origin.
    /// </summary>
    public Vector2 Position = position;

    /// <summary>
    /// Generates the rectangular hitbox for the <see cref="Collider"/>.
    /// </summary>
    /// <param name="width">The width, in pixels, of the hitbox.</param>
    /// <param name="height">The height, in pixels, of the hitbox.</param>
    /// <param name="alignment">How the hitbox should be positioned with respect to the Position of the <see cref="Collider"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void GenerateHitbox(int width, int height, Alignment alignment = Alignment.TopLeft)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("The width and height of the hitbox must be greater than zero.");
        }

        Hitbox = new(width, height, alignment);
        UpdateHitbox(Position);
    }

    /// <summary>
    /// Updates the rectangular region representing the Collider's hitbox to the provided position.
    /// </summary>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public void UpdateHitbox(Vector2 position)
    {
        int width = Hitbox.Width;
        int height = Hitbox.Height;

        Rectangle hitbox = Hitbox.Alignment switch
        {
            Alignment.TopLeft => new((int)position.X, (int)position.Y, width, height),
            Alignment.Top => new((int)position.X - width / 2, (int)position.Y, width, height),
            Alignment.TopRight => new((int)position.X - width, (int)position.Y, width, height),

            Alignment.Left => new((int)position.X, (int)position.Y - height / 2, width, height),
            Alignment.Center => new((int)position.X - width / 2, (int)position.Y - height / 2, width, height),
            Alignment.Right => new((int)position.X - width, (int)position.Y - height / 2, width, height),

            Alignment.BottomLeft => new((int)position.X, (int)position.Y - height, width, height),
            Alignment.Bottom => new((int)position.X - width / 2, (int)position.Y - height, width, height),
            Alignment.BottomRight => new((int)position.X - width, (int)position.Y - height, width, height),

            _ => throw new InvalidEnumArgumentException()
        };

        Hitbox.LoadHitbox(hitbox);
    }

    /// <summary>
    /// Updates the rectangular region representing the Collider's hitbox to the provided X and Y position.
    /// </summary>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public void UpdateHitbox(float X, float Y)
    {
        UpdateHitbox(new Vector2(X, Y));
    }

    public static bool AABB(Rectangle hitbox, Rectangle obj)
    {
        return obj.Intersects(hitbox);
    }
    public static bool SweptAABB(Rectangle currentHitbox, Rectangle newHitbox, Rectangle obj)
    {
        Rectangle union = Rectangle.Union(currentHitbox, newHitbox);
        return obj.Intersects(union);
    }
}