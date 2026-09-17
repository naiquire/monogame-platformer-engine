using System;

namespace MonoLibrary.Structures;
public class Circle : Polygon
{
    public float Radius { get; private set; }
    public float Diameter => 2 * Radius;

    public override Vector2 Center => new(X + Radius, Y + Radius);
    public override Vector2 BoundingSize => new(Diameter);
    public override bool IsEmpty => X == 0 && Y == 0 & Radius == 0;

    public override float Left => X;
    public override float Right => X + Diameter;
    public override float Top => Y;
    public override float Bottom => Y + Diameter;

    public Circle(float x, float y, float radius) : base(x, y)
    {
        Radius = radius;
    }
    public Circle(Vector2 position, float radius) : base(position)
    {
        Radius = radius;
    }
    public Circle(Vector2 position, Vector2 boundingSize) : base(position)
    {
        if (boundingSize.X != boundingSize.Y) throw new ArgumentException("X and Y must be equal");
        Radius = boundingSize.X;
    }

    public static bool operator ==(Circle a, Circle b)
    {
        return a.X == b.X && a.Y == b.Y && a.Radius == b.Radius;
    }
    public static bool operator !=(Circle a, Circle b) => !(a == b);

    public override bool Equals(object obj) => obj is Circle c && Equals(c);
    public override bool Equals(Polygon obj) => obj is Circle c && Equals(c);
    public bool Equals(Circle obj) => X == obj.X && Y == obj.Y && Radius == obj.Radius;
    public override int GetHashCode() => HashCode.Combine(X, Y, Radius);

    public override bool Contains(float x, float y)
    {
        return Vector2.DistanceSquared(Position, new(x, y)) < Radius * Radius;
    }

    public void Inflate(float amount)
    {
        Radius += amount;
    }
    public override void Inflate(float x, float y)
    {
        if (x != y) throw new ArgumentException("X and Y must be equal");
        Inflate(x);
    }

    public override bool Intersects(Quadrilateral rectangle)
    {
        throw new NotImplementedException();
    }
    public override bool Intersects(RightTriangle triangle)
    {
        throw new NotImplementedException();
    }
    public override bool Intersects(Circle circle)
    {
        float radiiSquared = (Radius + circle.Radius) * (Radius + circle.Radius);
        float distanceSquared = Vector2.DistanceSquared(Position, circle.Position);
        return distanceSquared < radiiSquared;
    }
}