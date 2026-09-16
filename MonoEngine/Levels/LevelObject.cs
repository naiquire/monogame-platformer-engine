using MonoEngine.Entities;
using MonoLibrary.Colliders;

namespace MonoEngine.Levels;
public abstract class LevelObject
{
    public abstract void HandleCollision(Entity entity);
}
public abstract class LevelObject<T> : LevelObject where T : Polygon
{
    public Collider<T> Collider;
}