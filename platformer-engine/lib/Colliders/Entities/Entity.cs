using Microsoft.Xna.Framework;
using lib.Graphics.Sprites;
using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using lib.Scenes;

namespace lib.Entities;

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}
public enum Alignment
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Center,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}
public struct EntityHitbox(int width, int height, Alignment alignment)
{
    public Rectangle Hitbox { get; private set; }
    public readonly int Width = width;
    public readonly int Height = height;
    public readonly Alignment Alignment = alignment;

    /// <summary>
    /// Assigns a rectangular region to be the hitbox of the <see cref="Entity"/>.
    /// </summary>
    /// <param name="hitbox"></param>
    public void LoadHitbox(Rectangle hitbox)
    {
        Hitbox = hitbox;
    }
}
public struct EntityTexture(Sprite texture)
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
    /// Invokes the drawing logic for the <see cref="Sprite"/>, and accountes for the offset of the sprite.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="position"></param>
    public readonly void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Texture.Draw(spriteBatch, position + SpriteOffset);
    }
}
public abstract class Entity(Vector2 position) : Collider(position), ICollidable
{
    /// <summary>
    /// The velocity of the <see cref="Entity"/>.
    /// </summary>
    protected Vector2 Velocity;

    /// <summary>
    /// Encapsulation for the properties and operation of the Entity's <see cref="Sprite"/>.
    /// </summary>
    protected EntityTexture Texture;



    public void LoadContent(Sprite texture)
    {
        Texture = new(texture);
    }

    public override void Update(GameTime gameTime, Scene scene)
    {
        Texture.Texture?.Update(gameTime);
    }
}