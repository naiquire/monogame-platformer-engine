using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using lib.Graphics;
using lib.Graphics.Sprites;
using lib.Structures;
using platformer_engine;
using lib;
using lib.Input;

namespace Entities;

class Player
{
    private readonly Vector2 _hitboxSize = new(60, 60);
    private Vector2 _spriteOffset;

    public Sprite Texture { get; private set; }
    public Vector2 Position { get; private set; } // bottom center
    public Rectangle Hitbox { get; private set; }
    public Vector2 Velocity { get; private set; }

    public Player()
    {
        Position = new(100, 100);
        UpdateHitbox();
    }

    public void LoadContent(Sprite texture)
    {
        Texture = texture;
    }
    public void LoadContent(AnimatedSprite texture)
    {
        Texture = texture;
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        Vector2 currentPosition = Position;

        UpdatePosition(keyboardState);
        UpdateHitbox();

        if (!Core.ScreenBounds.Contains(Hitbox))
        {
            Position = currentPosition;
            UpdateHitbox();
            Velocity = Vector2.Zero;
        }

        Texture.Update(gameTime);    
    }

    private void UpdatePosition(KeyboardState keyboardState)
    {
        Vector2 acceleration = Vector2.Zero;
        const float accelerationFactor = 1.0f;

        if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
        {
            acceleration.X += accelerationFactor;
        }
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            acceleration.X -= accelerationFactor;
        }
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            acceleration.Y -= accelerationFactor;
        }
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            acceleration.Y += accelerationFactor;
        }

        Velocity += acceleration;
        Position += Velocity;
    }
    private void UpdateHitbox()
    {
        Hitbox = new(
            (int) (Position.X - _hitboxSize.X * 0.5f),
            (int) (Position.Y - _hitboxSize.Y),
            (int) _hitboxSize.X,
            (int) _hitboxSize.Y
        );
    }

    public Vector2 GetSpritePosition()
    {
        return new Vector2(
            Position.X - Texture.Width * 0.5f,
            Position.Y - Texture.Height
        );
    }
}