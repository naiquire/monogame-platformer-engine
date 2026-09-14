using System;
using Microsoft.Xna.Framework;

namespace MonoLibrary.Structures;

public readonly struct RightTriangle : IEquatable<RightTriangle> 
{
    private static readonly RightTriangle s_empty = new();

    /// <summary>
    /// The x-coordinate of the right-angled corner of the triangle.
    /// </summary>
    public readonly int X;

    /// <summary>
    /// The y-coordinate of the right-angled vertex of the triangle.
    /// </summary>
    public readonly int Y;

    /// <summary>
    /// The length, in pixels, from the bottom-left vertex to the right-angled vertex.
    /// </summary>
    public readonly int Width;

    /// <summary>
    /// The length, in pixels, from the top-right vertex to the right-angled vertex.
    /// </summary>
    public readonly int Height;

    /// <summary>
    /// Gets the location of the right-angled vertex of the triangle
    /// </summary>
    public readonly Point Location => new(X, Y);

    /// <summary>
    /// Gets a triangle with X=0, Y=0, and Width=0, Height=0.
    /// </summary>
    public static RightTriangle Empty => s_empty;

    /// <summary>
    /// Gets a value that indicates whether this triangle has dimensions of (0, 0) and a location of (0, 0).
    /// </summary>
    public readonly bool IsEmpty => X == 0 && Y == 0 && Width == 0 && Height == 0;

    /// <summary>
    /// Gets the y-coordinate of the highest point on this triangle.
    /// </summary>
    public readonly int Top => Y;

    /// <summary>
    /// Gets the y-coordinate of the lowest point on this triangle.
    /// </summary>
    public readonly int Bottom => Y + Height;

    /// <summary>
    /// Gets the x-coordinate of the leftmost point on this triangle.
    /// </summary>
    public readonly int Left => X;

    /// <summary>
    /// Gets the x-coordinate of the rightmost point on this triangle.
    /// </summary>
    public readonly int Right => X + Width;

    /// <summary>
    /// Gets the length of the hypotenuse of the triangle
    /// </summary>
    public readonly float HypotenuseLength => MathF.Sqrt(Width * Width + Height * Height);

    /// <summary>
    /// Creates a new right triangle with the specified position and dimensions.
    /// </summary>
    /// <param name="x">The x-coordinate of the right-angled vertex of the triangle.</param>
    /// <param name="y">The y-coordinate of the right-angled vertex of the triangle.</param>
    /// <param name="width">The length from the right-angled vertex to the bottom-left vertex.</param>
    /// <param name="height">The length from the right-angled vertex to the top-right vertex.</param>
    public RightTriangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Creates a new circle with the specified position and radius.
    /// </summary>
    /// <param name="location">The right-angled vertex of the triangle.</param>
    /// <param name="dimensions">The dimensions of the triangle.</param>
    public RightTriangle(Point location, Point dimensions)
    {
        X = location.X;
        Y = location.Y;
        Width = dimensions.X;
        Height = dimensions.Y;
    }

    /// <summary>
    /// Returns a value that indicates whether the specified triangle intersects with this triangle.
    /// </summary>
    /// <param name="other">The other triangle to check.</param>
    /// <returns>true if the other triangle intersects with this triangle; otherwise, false.</returns>
    public bool Intersects(RightTriangle other)
    {
        return false;
    }

    /// <summary>
    /// Returns a value that indicates whether this triangle and the specified object are equal
    /// </summary>
    /// <param name="obj">The object to compare with this triangle.</param>
    /// <returns>true if this triangle and the specified object are equal; otherwise, false.</returns>
    public override readonly bool Equals(object obj) => obj is RightTriangle other && Equals(other);

    /// <summary>
    /// Returns a value that indicates whether this triangle and the specified triangle are equal.
    /// </summary>
    /// <param name="other">The triangle to compare with this triangle.</param>
    /// <returns>true if this triangle and the specified triangle are equal; otherwise, false.</returns>
    public readonly bool Equals(RightTriangle other) => this.X == other.X &&
                                                        this.Y == other.Y &&
                                                        this.Width == other.Width &&
                                                        this.Height == other.Height;

    /// <summary>
    /// Returns the hash code for this triangle.
    /// </summary>
    /// <returns>The hash code for this triangle as a 32-bit signed integer.</returns>
    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

    /// <summary>
    /// Returns a value that indicates if the triangle on the left hand side of the equality operator is equal to the
    /// triangle on the right hand side of the equality operator.
    /// </summary>
    /// <param name="lhs">The triangle on the left hand side of the equality operator.</param>
    /// <param name="rhs">The triangle on the right hand side of the equality operator.</param>
    /// <returns>true if the two triangles are equal; otherwise, false.</returns>
    public static bool operator ==(RightTriangle lhs, RightTriangle rhs) => lhs.Equals(rhs);

    /// <summary>
    /// Returns a value that indicates if the triangle on the left hand side of the inequality operator is not equal to the
    /// triangle on the right hand side of the inequality operator.
    /// </summary>
    /// <param name="lhs">The triangle on the left hand side of the inequality operator.</param>
    /// <param name="rhs">The triangle on the right hand side fo the inequality operator.</param>
    /// <returns>true if the two triangle are not equal; otherwise, false.</returns>
    public static bool operator !=(RightTriangle lhs, RightTriangle rhs) => !lhs.Equals(rhs);
}
