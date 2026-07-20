using Microsoft.Xna.Framework;
using lib.Entities;
using lib.Scenes;

public class LevelObject(Vector2 position) : Collider(position), ICollidable
{
    public override void Update(GameTime gameTime, Scene scene)
    {
        
    }
}