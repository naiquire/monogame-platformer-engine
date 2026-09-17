using System;

namespace MonoLibrary.Structures;
public class RightTriangle : Quadrilateral
{
    public enum NormalDirection
    {
        UpLeft, UpRight, DownLeft, DownRight
    }

    public NormalDirection Direction;
    public Vector2 RightAngleVertex => new(Right, Bottom);
    public float HypotenuseLength => MathF.Sqrt(Width * Width + Height * Height);
    
    public RightTriangle(float x, float y, float width, float height, NormalDirection direction) : base(x, y, width, height)
    {
        Direction = direction;
    }
    public RightTriangle(Vector2 position, Vector2 boundingSize, NormalDirection direction) : base(position, boundingSize)
    {
        Direction = direction;
    }

    public override bool Equals(object obj) => obj is RightTriangle t && Equals(t);
    public override bool Equals(Polygon obj) => obj is RightTriangle t && Equals(t);
    public bool Equals(RightTriangle obj) => X == obj.X && Y == obj.Y && Width == obj.Width && Height == obj.Height;
    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height, Direction);

    public override bool Intersects(Quadrilateral rectangle)
    {
        return base.Intersects(rectangle);
    }
    public override bool Intersects(RightTriangle triangle)
    {
        return base.Intersects(triangle);
    }
    public override bool Intersects(Circle circle)
    {
        return base.Intersects(circle);
    }
}