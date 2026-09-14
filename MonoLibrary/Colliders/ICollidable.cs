using Microsoft.Xna.Framework;
using MonoLibrary.Scenes;
using MonoLibrary.Colliders.Rectangles;

namespace MonoLibrary.Colliders;
public interface ICollidable
{
    Rectangle GetHitbox();
    void GenerateHitbox(int width, int height, Alignment alignment);
    void UpdateHitbox(Vector2 position);
    void UpdateHitbox(float X, float Y);
}