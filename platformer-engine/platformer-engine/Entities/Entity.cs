using Microsoft.Xna.Framework;
using lib.Graphics.Sprites;
using lib.Colliders;
using platformer_engine;

namespace Entities;
public abstract class Entity(LevelScene scene, Vector2 position) : Collider(position), ICollidable
{
    /// <summary>
    /// The velocity of the <see cref="Entity"/>.
    /// </summary>
    protected Vector2 Velocity;

    /// <summary>
    /// Encapsulation for the properties and operation of the Entity's <see cref="Sprite"/>.
    /// </summary>
    protected TextureManager Texture;

    protected LevelScene Scene { get; } = scene;

    public void LoadContent(Sprite texture)
    {
        Texture = new(texture);
    }

    public virtual void Update(GameTime gameTime)
    {
        Texture.Texture?.Update(gameTime);
    }
}