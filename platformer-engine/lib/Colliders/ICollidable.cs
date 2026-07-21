using Microsoft.Xna.Framework;
using lib.Scenes;

namespace lib.Colliders;
public interface ICollidable
{
    Rectangle GetHitbox();
    void GenerateHitbox(int width, int height, Alignment alignment);
    void UpdateHitbox();
    void Update(GameTime gameTime, Scene scene);
}