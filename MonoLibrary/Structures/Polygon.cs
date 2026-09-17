using System;

namespace MonoLibrary.Structures;
public abstract class Polygon : IEquatable<Polygon>
{
    public float X { get; protected set; }
    public float Y { get; protected set; }
    public float Width => Right - Left;
    public float Height => Bottom - Top;

    public Vector2 Position => new(X, Y);
    public abstract Vector2 Center { get; }
    public abstract Vector2 BoundingSize { get; }

    public abstract float Left { get; }
    public abstract float Right { get; }
    public abstract float Top { get; }
    public abstract float Bottom { get; }

    public abstract bool IsEmpty { get; }
    public abstract bool Equals(Polygon obj);

    public Polygon(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Polygon(Vector2 position)
    {
        X = position.X;
        Y = position.Y;
    }

    public void Offset(float x, float y)
    {
        X += x;
        Y += y;
    }
    public void Offset(Vector2 position) => Offset(position.X, position.Y);

    public abstract bool Contains(float x, float y);
    public bool Contains(Vector2 position) => Contains(position.X, position.Y);

    public abstract void Inflate(float x, float y);
    public void Inflate(Vector2 position) => Inflate(position.X, position.Y);

    public abstract bool Intersects(Quadrilateral rectangle);
    public abstract bool Intersects(RightTriangle triangle);
    public abstract bool Intersects(Circle circle);
}