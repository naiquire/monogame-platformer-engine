using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using lib.Graphics;
using lib.Graphics.Sprites;
using lib.Structures;
using platformer_engine;
using lib;
using lib.Input;
using lib.Entities;
using lib.Scenes;

namespace Entities;

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

public record Cheats
{
    public bool Noclip;
    public bool InfiniteJump;
    public bool InfiniteDash;
}

class Player : Entity
{
    private readonly PlayerState _playerState;
    private readonly Cheats _cheats;
    private readonly Vector2 _gravity;

    public Player() : base(new Vector2(200, 200))
    {
        _playerState = new()
        {
            State = State.Normal,
            DirectionFacing = Direction.Right,
            IsAirborne = true,
            CanDash = true
        };
        _cheats = new()
        {
            Noclip = false,
            InfiniteJump = false,
            InfiniteDash = true
        };

        _gravity = new(0, 0.5f);

        GenerateHitbox(30, 60, Alignment.Bottom);
    }

    public override void Update(GameTime gameTime, Scene scene)
    {
        
        CheckKeystrokes();

        switch (_playerState.State)
        {
            case State.Normal:
                Vector2 previousPosition = Position;
                Rectangle previousHitbox = GetHitbox();

                UpdateXVelocity();
                Position.X += Velocity.X;
                UpdateHitbox();

                if (AABB(previousHitbox, GetHitbox(), scene.LevelObjects[0].GetHitbox()))
                {
                    if (Velocity.X > 0)
                    {
                        float distance = Hitbox.Hitbox.Right - scene.LevelObjects[0].GetHitbox().Left;
                        Position.X -= distance;
                    }
                    if (Velocity.X < 0)
                    {
                        float distance = scene.LevelObjects[0].GetHitbox().Right - Hitbox.Hitbox.Left;
                        Position.X += distance;
                    }

                    Velocity.X = 0;
                    UpdateHitbox();
                }

                previousPosition = Position;
                previousHitbox = GetHitbox();

                UpdateYVelocity();
                Position.Y += Velocity.Y;
                UpdateHitbox();

                if (AABB(previousHitbox, GetHitbox(), scene.LevelObjects[0].GetHitbox()))
                {
                    if (Velocity.Y > 0)
                    {
                        float distance = Hitbox.Hitbox.Bottom - scene.LevelObjects[0].GetHitbox().Top;
                        Position.Y -= distance;
                    }
                    if (Velocity.Y < 0)
                    {
                        float distance = scene.LevelObjects[0].GetHitbox().Bottom - Hitbox.Hitbox.Top;
                        Position.Y += distance;
                    }

                    Velocity.Y = 0;
                    UpdateHitbox();
                }

                // System.Console.WriteLine($"{Position.X} {Position.Y}");

                break;
            case State.Dashing:
                UpdateDash(gameTime);
                Position += Velocity;
                UpdateHitbox();
                break;
            default:
                break;
        }

        _playerState.IsAirborne = CheckIfAirborne(scene);

        base.Update(gameTime, scene);
    }

    // private void HandleRoomCollision(Vector2 currentPosition)
    // {
    //     Rectangle roomBounds = new Rectangle();

    //     if (Hitbox.Left <= roomBounds.Left)
    //     {
    //         Velocity.X = 0;
    //         if (Velocity.Y > 0) Velocity.Y = 0;
    //         Position.X = currentPosition.X;
    //         UpdateHitbox();
    //     }
    //     if (Hitbox.Right >= roomBounds.Right)
    //     {
    //         Velocity.X = 0;
    //         if (Velocity.Y > 0) Velocity.Y = 0;
    //         Position.X = currentPosition.X;
    //         UpdateHitbox();
    //     }

    //     if (Hitbox.Top <= roomBounds.Top)
    //     {
    //         Velocity.Y = 0;
    //         Position.Y = currentPosition.Y;
    //         UpdateHitbox();
    //     }
    //     if (Hitbox.Bottom >= roomBounds.Bottom)
    //     {
    //         Velocity.Y = 0;
    //         Position.Y = currentPosition.Y;
    //         UpdateHitbox();
    //         _playerState.IsAirborne = false;
    //         _playerState.CanDash = true;
    //     }
    // }

    private void UpdateXVelocity()
    {
        const float x_speed = 10;

        // horizontal movement
        bool leftKeyPressed = Core.Input.Keyboard.IsKeyDown(Keys.Left);
        bool rightKeyPressed = Core.Input.Keyboard.IsKeyDown(Keys.Right);
        if (leftKeyPressed && !rightKeyPressed)
        {
            Velocity.X = -x_speed;
            _playerState.DirectionFacing = Direction.Left;
        }
        else if (!leftKeyPressed && rightKeyPressed)
        {
            Velocity.X = x_speed;
            _playerState.DirectionFacing = Direction.Right;
        }
        else
        {
            Velocity.X = 0;
        }
    }
    private void CheckKeystrokes()
    {
        // dashing
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.C))
        {
            if (_playerState.CanDash || _cheats.InfiniteDash)
            {
                Dash();
                _playerState.CanDash = false;
            }
        }
    }
    private void UpdateYVelocity()
    {
        const float y_speed = 10;

        // jumping
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Z))
        {
            if (!_playerState.IsAirborne || _cheats.InfiniteJump)
            {
                Velocity.Y = -y_speed;
                _playerState.IsAirborne = true;
            }
        }

        // handle gravity
        if (_playerState.IsAirborne) Velocity += _gravity;
        if (Velocity.Y > 20)
        {
            Velocity.Y = 20;
        }
    }

    private bool CheckIfAirborne(Scene scene)
    {
        Rectangle hitboxForFloorCollision = GetHitbox();
        hitboxForFloorCollision.Offset(0, 1);
        return !hitboxForFloorCollision.Intersects(scene.LevelObjects[0].GetHitbox());
    }
    
    private void Dash()
    {
        _playerState.State = State.Dashing;
        _playerState.DashTimeRemaining = 0.1f;
    }
    private void UpdateDash(GameTime gameTime)
    {
        const float dashSpeed = 50f;

        Velocity.Y = 0;
        if (_playerState.DirectionFacing == Direction.Left)
        {
            Velocity.X = -dashSpeed;
        }
        if (_playerState.DirectionFacing == Direction.Right)
        {
            Velocity.X = dashSpeed;
        }
        
        _playerState.DashTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;

        if (_playerState.DashTimeRemaining <= 0)
        {
            _playerState.State = State.Normal;
            _playerState.DashTimeRemaining = -1;
            _playerState.CanDash = true;
        }
    }


    public bool AABB(Rectangle currentHitbox, Rectangle newHitbox, Rectangle obj)
    {
        if (_cheats.Noclip) return false;

        Rectangle union = Rectangle.Union(currentHitbox, newHitbox);
        return obj.Intersects(union);
    }
}