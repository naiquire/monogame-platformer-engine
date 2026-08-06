using lib.Colliders;
using Microsoft.Xna.Framework;

namespace Objects;
public enum ObjectType
{
    Solid, Hazard, Trigger
}
public abstract class LevelObject(ObjectType type, Vector2 position) : Collider(position), ICollidable
{
    public ObjectType Type { get; } = type;
}





