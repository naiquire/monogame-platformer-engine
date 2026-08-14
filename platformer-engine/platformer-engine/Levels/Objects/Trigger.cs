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
