using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using lib.Graphics;
using lib.Graphics.Sprites;
using platformer_engine;
using System.ComponentModel;

namespace Entities;

struct State
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Rectangle Hitbox { get; set; }
}
class Player
{
    public AnimatedSprite Texture { get; private set; }
    private State PlayerState;
    
    public Vector2 GetPosition() => PlayerState.Position;
    public Vector2 GetVelocity() => PlayerState.Velocity;
    public Player()
    {
        PlayerState = new()
        {
            Position = new Vector2(100, 100),
            Velocity = Vector2.Zero
        };
    }

    public void LoadContent(AnimatedSprite texture)
    {
        Texture = texture;
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        State currentState = PlayerState;

        UpdatePosition(keyboardState);
        UpdateHitbox();
        Texture.Update(gameTime);

        if (PlayerOutOfBounds())
        {
            PlayerState = currentState;
            PlayerState.Velocity = Vector2.Zero;
        }        
    }

    private bool PlayerOutOfBounds()
    {
        return PlayerState.Hitbox.Left <= PlatformerEngine.ScreenBounds.Left ||
            PlayerState.Hitbox.Right >= PlatformerEngine.ScreenBounds.Right ||
            PlayerState.Hitbox.Top <= PlatformerEngine.ScreenBounds.Top ||
            PlayerState.Hitbox.Bottom >= PlatformerEngine.ScreenBounds.Bottom;

    }

    private void UpdatePosition(KeyboardState keyboardState)
    {
        Vector2 acceleration = Vector2.Zero;
        const float accelerationFactor = 1.0f;

        if (keyboardState.IsKeyDown(Keys.Right))
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

        PlayerState.Velocity += acceleration;
        PlayerState.Position += PlayerState.Velocity;
    }
    private void UpdateHitbox()
    {
        PlayerState.Hitbox = new Rectangle((int)GetPosition().X, (int)GetPosition().Y, (int)Texture.Width, (int)Texture.Height);
    }
}