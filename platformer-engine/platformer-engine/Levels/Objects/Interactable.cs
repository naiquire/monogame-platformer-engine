using Entities;
using lib.Colliders;
using Microsoft.Xna.Framework;

namespace Levels.Objects;
public abstract class Interactable(Vector2 position) : Collider(position)
{
    public abstract void HandleCollision(Player player);
}
