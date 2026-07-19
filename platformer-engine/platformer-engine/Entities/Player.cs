using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using lib.Graphics;
using lib.Graphics.Sprites;
using lib.Structures;
using platformer_engine;
using lib;
using lib.Input;
using MonoGame.Framework.Devices.Sensors;
using System.Runtime.CompilerServices;

namespace Entities;

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}
public enum State
{
    Normal,
    Dashing
}

public record PlayerState
{
    public State State;
    public Direction DirectionFacing;
    public double DashTimeRemaining;
    public bool IsAirborne;
    public bool CanDash;
}

class Player
{
    private readonly Vector2 _hitboxSize = new(60, 60);
    public Rectangle Hitbox { get; private set; }

    public Sprite Texture { get; private set; }
    public Vector2 Position; // bottom center
    private Vector2 _velocity;
    private readonly Vector2 _acceleration;

    private PlayerState _playerState;

    public Player()
    {
        _playerState = new()
        {
            State = State.Normal,
            DirectionFacing = Direction.Right
        };

        Position = new(200, 200);
        _velocity = Vector2.Zero;
        _acceleration = new(0, 0.5f);

        UpdateHitbox();
    }

    public void LoadContent(Sprite texture)
    {
        Texture = texture;
    }

    public void Update(GameTime gameTime, Rectangle roomBounds)
    {
        Vector2 currentPosition = Position;

        switch (_playerState.State)
        {
            case State.Normal:
                UpdatePosition();
                break;
            case State.Dashing:
                UpdateDash(gameTime);
                break;
            default:
                break;
        }

        Position += _velocity;
        UpdateHitbox();

        HandleRoomCollision(currentPosition, roomBounds);

        Texture?.Update(gameTime);
    }

    private void HandleRoomCollision(Vector2 currentPosition, Rectangle roomBounds)
    {
        if (Hitbox.Left <= roomBounds.Left)
        {
            _velocity.X = 0;
            if (_velocity.Y > 0) _velocity.Y = 0;
            Position.X = currentPosition.X;
            UpdateHitbox();
        }
        if (Hitbox.Right >= roomBounds.Right)
        {
            _velocity.X = 0;
            if (_velocity.Y > 0) _velocity.Y = 0;
            Position.X = currentPosition.X;
            UpdateHitbox();
        }

        if (Hitbox.Top <= roomBounds.Top)
        {
            _velocity.Y = 0;
            Position.Y = currentPosition.Y;
            UpdateHitbox();
        }
        if (Hitbox.Bottom >= roomBounds.Bottom)
        {
            _velocity.Y = 0;
            Position.Y = currentPosition.Y;
            UpdateHitbox();
            _playerState.IsAirborne = false;
            _playerState.CanDash = true;
        }
    }

    private void UpdatePosition()
    {
        const float x_speed = 10;
        const float y_speed = 10;


        // horizontal movement
        bool leftKeyPressed = Core.Input.Keyboard.IsKeyDown(Keys.Left);
        bool rightKeyPressed = Core.Input.Keyboard.IsKeyDown(Keys.Right);
        if (leftKeyPressed && !rightKeyPressed)
        {
            _velocity.X = -x_speed;
            _playerState.DirectionFacing = Direction.Left;
        }
        else if (!leftKeyPressed && rightKeyPressed)
        {
            _velocity.X = x_speed;
            _playerState.DirectionFacing = Direction.Right;
        }
        else
        {
            _velocity.X = 0;
        }

        // dashing
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.C))
        {
            if (_playerState.CanDash) Dash();
            _playerState.CanDash = false;
        }

        // jumping
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Z))
        {
            if (!_playerState.IsAirborne)
            {
                _velocity.Y = -y_speed;
                _playerState.IsAirborne = true;
            }
        }

        // handle gravity
        _velocity += _acceleration;
        if (_velocity.Y < -10)
        {
            _velocity.Y = -10;
        }
    }
    
    private void Dash()
    {
        _playerState.State = State.Dashing;
        _playerState.DashTimeRemaining = 0.1f;
    }
    private void UpdateDash(GameTime gameTime)
    {
        const float dashSpeed = 50f;

        _velocity.Y = 0;
        if (_playerState.DirectionFacing == Direction.Left)
        {
            _velocity.X = -dashSpeed;
        }
        if (_playerState.DirectionFacing == Direction.Right)
        {
            _velocity.X = dashSpeed;
        }
        
        _playerState.DashTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;

        if (_playerState.DashTimeRemaining <= 0)
        {
            _playerState.State = State.Normal;
            _playerState.DashTimeRemaining = -1;
        }
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