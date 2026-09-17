using MonoLibrary.Colliders;
using Microsoft.Xna.Framework;
using MonoEngine.Entities;
using MonoLibrary.Structures;

namespace MonoEngine.Levels.Objects;
public abstract class SolidObject<T> : LevelObject<T> where T : Polygon
{
    public override void HandleCollision(Entity entity)
    {
        
    }
}

public class BlockSolid : SolidObject<Quadrilateral>
{
    public BlockSolid(Vector2 position, Vector2 dimensions)
    {
        Collider = new RectangleCollider(position);
        ((RectangleCollider)Collider).GenerateHitbox(dimensions);
    }
}

public class SlopeSolid : SolidObject<RightTriangle>
{
    public SlopeSolid(Vector2 position, Vector2 dimensions, RightTriangle.NormalDirection direction)
    {
        Collider = new TriangleCollider(position);
        ((TriangleCollider)Collider).GenerateHitbox(dimensions, direction);
    }
}