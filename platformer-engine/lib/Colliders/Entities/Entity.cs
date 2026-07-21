using Microsoft.Xna.Framework;
using lib.Graphics.Sprites;
using lib.Scenes;

namespace lib.Colliders.Entities;
public abstract class Entity(Vector2 position) : Collider(position), ICollidable
{
    /// <summary>
    /// The velocity of the <see cref="Entity"/>.
    /// </summary>
    protected Vector2 Velocity;

    /// <summary>
    /// Encapsulation for the properties and operation of the Entity's <see cref="Sprite"/>.
    /// </summary>
    protected TextureManager Texture;

    public void LoadContent(Sprite texture)
    {
        Texture = new(texture);
    }

    public override void Update(GameTime gameTime, Scene scene)
    {
        Texture.Texture?.Update(gameTime);
    }
}