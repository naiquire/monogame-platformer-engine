using Microsoft.Xna.Framework;
using lib.Scenes;

namespace lib.Colliders.Objects;
public class LevelObject(Vector2 position) : Collider(position), ICollidable
{
    public override void Update(GameTime gameTime, Scene scene)
    {
        
    }
}