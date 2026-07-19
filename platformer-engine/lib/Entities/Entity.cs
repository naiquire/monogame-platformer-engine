using Microsoft.Xna.Framework;
using lib.Graphics.Sprites;
using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;

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
    TopMiddle,
    TopRight,
    MiddleLeft,
    Center,
    MiddleRight,
    BottomLeft,
    BottomMiddle,
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
public abstract class Entity(Vector2 position)
{
    /// <summary>
    /// Defines a rectangular region from which the <see cref="Entity"/> collisions are calculated with.
    /// </summary>
    protected EntityHitbox Hitbox;
    public Rectangle GetHitbox() => Hitbox.Hitbox;

    /// <summary>
    /// The position of the <see cref="Entity"/> with respect to the top-left corner.
    /// </summary>
    public Vector2 Position = position;

    /// <summary>
    /// The velocity of the <see cref="Entity"/>.
    /// </summary>
    protected Vector2 Velocity;

    /// <summary>
    /// Encapsulation for the properties and operation of the Entity's <see cref="Sprite"/>.
    /// </summary>
    protected EntityTexture Texture;

    /// <summary>
    /// Generates the rectangular hitbox for the <see cref="Entity"/>.
    /// </summary>
    /// <param name="width">The width, in pixels, of the hitbox.</param>
    /// <param name="height">The height, in pixels, of the hitbox.</param>
    /// <param name="alignment">How the hitbox should be positioned with respect to the Position of the <see cref="Entity"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void GenerateHitbox(int width, int height, Alignment alignment)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("The width and height of the hitbox must be greater than zero.");
        }

        Hitbox = new(width, height, alignment);
        UpdateHitbox();
    }

    /// <summary>
    /// Updates the rectangular region representing the Entity's hitbox to its current position.
    /// </summary>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public void UpdateHitbox()
    {
        int width = Hitbox.Width;
        int height = Hitbox.Height;

        Rectangle hitbox = Hitbox.Alignment switch
        {
            Alignment.TopLeft => new((int)Position.X, (int)Position.Y, width, height),
            Alignment.TopMiddle => new((int)Position.X - width / 2, (int)Position.Y, width, height),
            Alignment.TopRight => new((int)Position.X - width, (int)Position.Y, width, height),

            Alignment.MiddleLeft => new((int)Position.X, (int)Position.Y - height / 2, width, height),
            Alignment.Center => new((int)Position.X - width / 2, (int)Position.Y - height / 2, width, height),
            Alignment.MiddleRight => new((int)Position.X - width, (int)Position.Y - height / 2, width, height),

            Alignment.BottomLeft => new((int)Position.X, (int)Position.Y - height, width, height),
            Alignment.BottomMiddle => new((int)Position.X - width / 2, (int)Position.Y - height, width, height),
            Alignment.BottomRight => new((int)Position.X - width, (int)Position.Y - height, width, height),

            _ => throw new InvalidEnumArgumentException()
        };

        Hitbox.LoadHitbox(hitbox);
    }

    public void LoadContent(Sprite texture)
    {
        Texture = new(texture);
    }

    public virtual void Update(GameTime gameTime)
    {
        Texture.Texture?.Update(gameTime);
    }
}