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
public class TriggerObject(Vector2 position, TriggerType type) : Interactable(position)
{
    public TriggerType Type = type;

    public static TriggerObject Build(Vector2 position, Vector2 dimensions, TriggerType type, Alignment alignment = Alignment.TopLeft)
    {
        var collider = new TriggerObject(position, type);
        collider.GenerateHitbox((int)dimensions.X, (int)dimensions.Y, alignment);
        return collider;
    }
    public override void HandleCollision(Player player)
    {
        switch (Type)
        {
            case TriggerType.Transition:
                Core.ChangeScene(new LevelScene("Levels/0.json", "Levels/tilemap.json"));
                break;
        }
    }
}
