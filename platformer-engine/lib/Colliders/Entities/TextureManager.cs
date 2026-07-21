using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using lib.Graphics.Sprites;

namespace lib.Colliders.Entities;
public struct TextureManager(Sprite texture)
{
    public readonly Sprite Texture = texture;

    /// <summary>
    /// Defines a vector which offsets the drawing origin of the <see cref="Sprite"/>
    /// </summary>
    public Vector2 SpriteOffset { get; private set; }

    /// <summary>
    /// Applies an offset to drawing the <see cref="Sprite"/>, such that its origin is different to the position of the encapsulating <see cref="Entity"/>.
    /// </summary>
    /// <param name="offset">A vector which maps the position of the <see cref="Entity"/> to the drawing origin of the <see cref="Sprite"/>.</param>
    public void SetSpriteOffset(Vector2 offset)
    {
        SpriteOffset = offset;
    }

    /// <summary>
    /// Invokes the drawing logic for the <see cref="Sprite"/>, and accounts for the offset of the sprite.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="position"></param>
    public readonly void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Texture.Draw(spriteBatch, position + SpriteOffset);
    }
}