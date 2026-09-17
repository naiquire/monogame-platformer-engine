using MonoLibrary.Colliders;
using Microsoft.Xna.Framework;
using MonoEngine.Entities;
using MonoLibrary.Structures;

namespace MonoEngine.Levels.Objects;
public abstract class HazardObject<T> : LevelObject<T> where T : Polygon
{
    public override void HandleCollision(Entity entity)
    {
        
    }
}

public class SpikeHazard : HazardObject<Quadrilateral>
{
    public SpikeHazard(Vector2 position, Vector2 dimensions)
    {
        Collider = new RectangleCollider(position);
        ((RectangleCollider)Collider).GenerateHitbox(dimensions);
    }
}

public class SawbladeHazard : HazardObject<Circle>
{
    public SawbladeHazard(Vector2 position, float radius)
    {
        Collider = new CircleCollider(position);
        ((CircleCollider)Collider).GenerateHitbox(radius);
    }
}