using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoEngine.Entities;
using MonoLibrary.Colliders;
using MonoLibrary.Structures;

namespace MonoEngine.Levels.Objects;

public enum TriggerType
{
    Transition
}

public abstract class TriggerObject(TriggerType type) : LevelObject<Quadrilateral>
{
    public TriggerType Type { get; } = type;

    public static TriggerObject Build(Vector2 position, Vector2 dimensions, TriggerType type)
    {
        return type switch
        {
            TriggerType.Transition => new TransitionTrigger(position, dimensions, type),
            _ => throw new InvalidEnumArgumentException()
        };
    }
}

public class TransitionTrigger : TriggerObject
{
    private readonly string _level = "Levels/0.json";
    private readonly string _tilemap = "Levels/tilemap.json";

    public TransitionTrigger(Vector2 position, Vector2 dimensions, TriggerType type) : base(type)
    {
        Collider = new RectangleCollider(position);
        ((RectangleCollider)Collider).GenerateHitbox(dimensions);
    }

    public override void HandleCollision(Entity entity)
    {
        
    }
}
