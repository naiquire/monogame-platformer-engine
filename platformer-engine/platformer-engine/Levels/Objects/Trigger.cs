using System.ComponentModel;
using Entities;
using lib;
using lib.Colliders;
using Microsoft.Xna.Framework;
using platformer_engine;

namespace Levels.Objects;

public enum TriggerType
{
    Transition
}
public abstract class TriggerObject(Vector2 position) : Interactable(position)
{
    public static TriggerObject CreateTriggerType(Vector2 position, TriggerType type)
    {
        return type switch
        {
            TriggerType.Transition => new TransitionTrigger(position),

            _ => throw new InvalidEnumArgumentException()
        };
    }
    public static TriggerObject Build(Vector2 position, Vector2 dimensions, TriggerType type, Alignment alignment = Alignment.TopLeft)
    {
        TriggerObject collider = CreateTriggerType(position, type);
        collider.GenerateHitbox((int)dimensions.X, (int)dimensions.Y, alignment);
        return collider;
    }

    public abstract void HandleCollision(Player player);
}

public class TransitionTrigger(Vector2 position) : TriggerObject(position)
{
    private readonly string _level = "Levels/0.json";
    private readonly string _tilemap = "Levels/tilemap.json";
    public override void HandleCollision(Player player)
    {
        Core.ChangeScene(new LevelScene(_level, _tilemap));
    }
}
