using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

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

public sealed class RectangleCollider(Vector2 position) : Collider<Quadrilateral>(position)
{
    public void GenerateHitbox(Vector2 dimensions, Alignment alignment = Alignment.TopLeft) => GenerateHitbox(dimensions.X, dimensions.Y, alignment);
    public void GenerateHitbox(float width, float height, Alignment alignment = Alignment.TopLeft)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("The width and height of the hitbox must be greater than zero.");
        }

        HitboxManager = new HitboxManager<Quadrilateral>(alignment);
        Vector2 alignedPosition = GetAlignedPosition(width, height);

        Quadrilateral hitbox = new(alignedPosition.X, alignedPosition.Y, width, height);
        HitboxManager.LoadHitbox(hitbox);
    }

    public override void UpdateHitbox(Vector2 position)
    {
        Position = position;
        Vector2 alignedPosition = GetAlignedPosition(Hitbox.BoundingSize);

        Quadrilateral hitbox = new(alignedPosition, Hitbox.BoundingSize);
        HitboxManager.LoadHitbox(hitbox);
    }
    public override void UpdateHitbox(float X, float Y) => UpdateHitbox(new(X, Y));

    public override bool SweptAABB(Quadrilateral currentHitbox, Quadrilateral newHitbox, Polygon obj)
    {
        Quadrilateral union = Quadrilateral.Union(currentHitbox, newHitbox);
        return obj.Intersects(union);
    }
}

public sealed class TriangleCollider(Vector2 position) : Collider<RightTriangle>(position)
{
    public void GenerateHitbox(Vector2 dimensions, RightTriangle.NormalDirection direction, Alignment alignment = Alignment.TopLeft) => GenerateHitbox(dimensions.X, dimensions.Y, direction, alignment);
    public void GenerateHitbox(float width, float height, RightTriangle.NormalDirection direction, Alignment alignment = Alignment.TopLeft)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("The width and height of the hitbox must be greater than zero.");
        }

        HitboxManager = new HitboxManager<RightTriangle>(alignment);
        Vector2 alignedPosition = GetAlignedPosition(width, height);

        RightTriangle hitbox = new(alignedPosition.X, alignedPosition.Y, width, height, direction);
        HitboxManager.LoadHitbox(hitbox);
    }

    public override void UpdateHitbox(Vector2 position)
    {
        Position = position;
        Vector2 alignedPosition = GetAlignedPosition(Hitbox.BoundingSize);
        
        RightTriangle hitbox = new(alignedPosition, Hitbox.BoundingSize, Hitbox.Direction);
        HitboxManager.LoadHitbox(hitbox);
    }
    public override void UpdateHitbox(float X, float Y) => UpdateHitbox(new(X, Y));

    public override bool SweptAABB(RightTriangle currentHitbox, RightTriangle newHitbox, Polygon obj)
    {
        throw new NotImplementedException();
    }
}

public sealed class CircleCollider(Vector2 position) : Collider<Circle>(position)
{
    public void GenerateHitbox(float radius, Alignment alignment = Alignment.TopLeft)
    {
        if (radius <= 0)
        {
            throw new ArgumentOutOfRangeException("The radius of the hitbox must be greater than zero.");
        }

        HitboxManager = new HitboxManager<Circle>(alignment);
        Vector2 alignedPosition = GetAlignedPosition(new(2 * radius));

        Circle hitbox = new(alignedPosition, radius);
        HitboxManager.LoadHitbox(hitbox);
    }

    public override void UpdateHitbox(Vector2 position)
    {
        Position = position;
        Vector2 alignedPosition = GetAlignedPosition(Hitbox.Diameter, Hitbox.Diameter);
        
        Circle hitbox = new(alignedPosition, Hitbox.Radius);
        HitboxManager.LoadHitbox(hitbox);
    }
    public override void UpdateHitbox(float X, float Y) => UpdateHitbox(new(X, Y));

    public override bool SweptAABB(Circle currentHitbox, Circle newHitbox, Polygon obj)
    {
        if (obj.Intersects(currentHitbox) || obj.Intersects(newHitbox)) return true;

        // vertically aligned
        if (currentHitbox.X == newHitbox.X)
        {
            var fill = new Quadrilateral(currentHitbox.Left, Math.Min(currentHitbox.Center.Y, newHitbox.Center.Y),
                currentHitbox.Diameter, Math.Abs(currentHitbox.Center.Y - newHitbox.Center.Y));
            return obj.Intersects(fill);
        }

        // horizontally aligned
        else if (currentHitbox.Y == newHitbox.Y)
        {
            var fill = new Quadrilateral(Math.Min(currentHitbox.Center.X, newHitbox.Center.X), currentHitbox.Top,
                Math.Abs(currentHitbox.Center.X - newHitbox.Center.X), currentHitbox.Diameter);
            return obj.Intersects(fill);
        }

        else
        {
            throw new ArgumentException("Swept AABB can only be calculated during motions in one dimension");
        }
    }
}


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