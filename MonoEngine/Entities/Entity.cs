using Microsoft.Xna.Framework;
using MonoLibrary.Graphics.Sprites;
using MonoLibrary.Colliders;
using MonoLibrary.Colliders.Rectangles;
using MonoLibrary.Scenes;

namespace MonoEngine.Entities;
public abstract class Entity(Scene scene, Vector2 position) : Collider(position), ICollidable
{
    /// <summary>
    /// The velocity of the <see cref="Entity"/>.
    /// </summary>
    protected Vector2 Velocity;

    /// <summary>
    /// Encapsulation for the properties and operation of the Entity's <see cref="Sprite"/>.
    /// </summary>
    protected TextureManager Texture;

    protected Scene Scene { get; } = scene;

    public void LoadContent(Sprite texture)
    {
        Texture = new(texture);
    }

    public virtual void Update(GameTime gameTime)
    {
        Texture.Texture?.Update(gameTime);
    }
}