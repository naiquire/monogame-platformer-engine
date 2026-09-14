using MonoLibrary.Structures;
using Microsoft.Xna.Framework;

namespace MonoLibrary.Colliders.Triangles;
public struct HitboxManager(int width, int height)
{
    /// <summary>
    /// The current hitbox of the <see cref="Entity"/>.
    /// </summary>
    public RightTriangle Hitbox { get; private set; }

    /// <summary>
    /// The hitbox of the <see cref="Entity"/> on the previous frame.
    /// </summary>
    public RightTriangle PreviousHitbox { get; private set; }

    /// <summary>
    /// The width, in pixels, of the hitbox.
    /// </summary>
    public readonly int Width = width;

    /// <summary>
    /// The height, in pixels, of the hitbox.
    /// </summary>
    public readonly int Height = height;

    /// <summary>
    /// Assigns a rectangular region to be the hitbox of the <see cref="Entity"/>.
    /// </summary>
    /// <param name="hitbox"></param>
    public void LoadHitbox(RightTriangle hitbox)
    {
        PreviousHitbox = Hitbox;
        Hitbox = hitbox;
    }
}