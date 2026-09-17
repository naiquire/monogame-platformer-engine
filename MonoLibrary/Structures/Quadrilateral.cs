using System;

namespace MonoLibrary.Structures;
public class Quadrilateral : Polygon
{
    public new float Width { get; private set; }
    public new float Height { get; private set; }

    public override Vector2 Center => new(X + Width / 2, Y + Height / 2);
    public override Vector2 BoundingSize => new(Width, Height);
    public override bool IsEmpty => X == 0 && Y == 0 && Width == 0 && Height == 0;

    public override float Left => X;
    public override float Right => X + Width;
    public override float Top => Y;
    public override float Bottom => Y + Height;

    public Quadrilateral(float x, float y, float width, float height) : base(x, y)
    {
        Width = width;
        Height = height;
    }
    public Quadrilateral(Vector2 position, Vector2 boundingSize) : base(position)
    {
        Width = boundingSize.X;
        Height = boundingSize.Y;
    }
    public static Quadrilateral FromRectangle(Rectangle r)
    {
        return new Quadrilateral(r.X, r.Y, r.Width, r.Height);
    }

    public static bool operator ==(Quadrilateral a, Quadrilateral b)
    {
        return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
    }
    public static bool operator !=(Quadrilateral a, Quadrilateral b) => !(a == b);

    public override bool Equals(object obj) => obj is Quadrilateral r && Equals(r);
    public override bool Equals(Polygon obj) => obj is Quadrilateral r && Equals(r);
    public bool Equals(Quadrilateral obj) => X == obj.X && Y == obj.Y && Width == obj.Width && Height == obj.Height;
    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

    public override bool Contains(float x, float y)
    {
        return Left <= x && x <= Right && Top <= y && y <= Bottom;
    }

    public override void Inflate(float x, float y)
    {
        X -= x;
        Y -= y;
        Width += x * 2;
        Height += y * 2;
    }

    public static Quadrilateral Union(Quadrilateral a, Quadrilateral b)
    {
        float num = Math.Min(a.X, b.X);
        float num2 = Math.Min(a.Y, b.Y);
        return new Quadrilateral(num, num2, Math.Max(a.Right, b.Right) - num, Math.Max(a.Bottom, b.Bottom) - num2);
    }

    public override bool Intersects(Quadrilateral rectangle)
    {
        return rectangle.Left < Right && Left < rectangle.Right && rectangle.Top < Bottom && Top < rectangle.Bottom;
    }
    public override bool Intersects(RightTriangle triangle)
    {
        throw new NotImplementedException();
    }
    public override bool Intersects(Circle circle)
    {
        throw new NotImplementedException();
    }
}