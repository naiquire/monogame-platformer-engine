using Entities;
using lib.Colliders;
using Microsoft.Xna.Framework;

namespace Levels.Objects;
public class SolidObject(Vector2 position) : Interactable(position)
{
    public override void HandleCollision(Player player)
    {
        
    }
}