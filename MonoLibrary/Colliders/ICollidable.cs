using Microsoft.Xna.Framework;

namespace MonoLibrary.Colliders;
public interface ICollidable
{
    Quadrilateral GetHitbox();
    void GenerateHitbox(int width, int height, Alignment alignment);
    void UpdateHitbox(Vector2 position);
    void UpdateHitbox(float X, float Y);
}