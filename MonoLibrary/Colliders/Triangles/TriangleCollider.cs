using System;
using MonoLibrary.Structures;
using Microsoft.Xna.Framework;

namespace MonoLibrary.Colliders.Triangles;
public class Collider(Vector2 position)
{
    /// <summary>
    /// Defines a triangular region from which collisions are calculated with.
    /// </summary>
    protected HitboxManager Hitbox;
    public RightTriangle GetHitbox() => Hitbox.Hitbox;
    public RightTriangle GetPreviousHitbox() => Hitbox.PreviousHitbox;

    /// <summary>
    /// The position of the <see cref="Collider"/> with respect to the origin.
    /// </summary>
    public Vector2 Position = position;

    /// <summary>
    /// Generates the triangular hitbox for the <see cref="Collider"/>.
    /// </summary>
    /// <param name="width">The width, in pixels, of the hitbox.</param>
    /// <param name="height">The height, in pixels, of the hitbox.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void GenerateHitbox(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("The width and height of the hitbox must be greater than zero.");
        }

        Hitbox = new(width, height);
        UpdateHitbox(Position);
    }

    /// <summary>
    /// Updates the triangular region representing the Collider's hitbox to the provided position.
    /// </summary>
    public void UpdateHitbox(Vector2 position)
    {
        int width = Hitbox.Width;
        int height = Hitbox.Height;

        RightTriangle hitbox = new((int)position.X, (int)position.Y, width, height);
        Hitbox.LoadHitbox(hitbox);
    }

    /// <summary>
    /// Updates the triangular region representing the Collider's hitbox to the provided X and Y position.
    /// </summary>
    public void UpdateHitbox(float X, float Y)
    {
        UpdateHitbox(new Vector2(X, Y));
    }
}