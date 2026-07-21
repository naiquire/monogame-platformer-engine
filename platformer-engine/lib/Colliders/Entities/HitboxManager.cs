using Microsoft.Xna.Framework;

namespace lib.Colliders.Entities;
public struct HitboxManager(int width, int height, Alignment alignment)
{
    /// <summary>
    /// The current hitbox of the <see cref="Entity"/>.
    /// </summary>
    public Rectangle Hitbox { get; private set; }

    /// <summary>
    /// The hitbox of the <see cref="Entity"/> on the previous frame.
    /// </summary>
    public Rectangle PreviousHitbox { get; private set; }

    /// <summary>
    /// The width, in pixels, of the hitbox.
    /// </summary>
    public readonly int Width = width;

    /// <summary>
    /// The height, in pixels, of the hitbox.
    /// </summary>
    public readonly int Height = height;

    /// <summary>
    /// How the hitbox is aligned with respect to the position of the <see cref="Entity"/>.
    /// </summary>
    public readonly Alignment Alignment = alignment;

    /// <summary>
    /// Assigns a rectangular region to be the hitbox of the <see cref="Entity"/>.
    /// </summary>
    /// <param name="hitbox"></param>
    public void LoadHitbox(Rectangle hitbox)
    {
        PreviousHitbox = Hitbox;
        Hitbox = hitbox;
    }
}