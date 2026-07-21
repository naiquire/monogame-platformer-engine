using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using lib;
using lib.Colliders.Entities;
using lib.Colliders;
using lib.Scenes;
using System.Diagnostics.Contracts;
using System.ComponentModel;

namespace Entities;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
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
    public bool IsCrouched;
}
public record Cheats
{
    public bool Noclip;
    public bool InfiniteJump;
    public bool InfiniteDash;
}

public class Player : Entity
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
            InfiniteDash = false
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

                UpdateXVelocity();
                Position.X += Velocity.X;
                UpdateHitbox();
                HandleHorizontalCollision(scene.LevelObjects);

                UpdateYVelocity();
                Position.Y += Velocity.Y;
                UpdateHitbox();
                HandleVerticalCollision(scene.LevelObjects);

                break;
            case State.Dashing:

                UpdateDash(gameTime);
                Position += Velocity;
                UpdateHitbox();
                HandleHorizontalCollision(scene.LevelObjects);
                
                break;
            default:
                break;
        }

        _playerState.IsAirborne = CheckIfAirborne(scene.LevelObjects);

        base.Update(gameTime, scene);
    }

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

        // crouch
        if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
        {
            if (!_playerState.IsAirborne)
            {
                Crouch();
            }
        }
        if (Core.Input.Keyboard.IsKeyUp(Keys.Down) && _playerState.IsCrouched)
        {
            Uncrouch();
        }
    }

    private void Crouch()
    {
        _playerState.IsCrouched = true;
        GenerateHitbox(30, 30, Hitbox.Alignment);
    }
    private void Uncrouch()
    {
        _playerState.IsCrouched = false;
        GenerateHitbox(30, 60, Hitbox.Alignment);
    }

    private void UpdateYVelocity()
    {
        const float y_speed = 10;

        if (_cheats.Noclip)
        {
            // vertical movement
            bool upKeyPressed = Core.Input.Keyboard.IsKeyDown(Keys.Up);
            bool downKeyPressed = Core.Input.Keyboard.IsKeyDown(Keys.Down);
            if (upKeyPressed && !downKeyPressed)
            {
                Velocity.Y = -y_speed;
            }
            else if (!upKeyPressed && downKeyPressed)
            {
                Velocity.Y = y_speed;
            }
            else
            {
                Velocity.Y = 0;
            }

            return;
        }

        // jumping
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Z))
        {
            if (!_playerState.IsAirborne || _cheats.InfiniteJump)
            {
                Velocity.Y = -y_speed;
                _playerState.IsAirborne = true;

                if (_playerState.IsCrouched) Uncrouch();
            }
        }

        // handle gravity
        if (_playerState.IsAirborne) Velocity += _gravity;
        if (Velocity.Y > 20)
        {
            Velocity.Y = 20;
        }
    }

    private void HandleHorizontalCollision(List<ICollidable> colliders)
    {
        foreach (ICollidable collider in colliders)
        {
            if (SweptAABB(GetPreviousHitbox(), GetHitbox(), collider.GetHitbox()))
            {
                if (Velocity.X > 0)
                {
                    float distance = Hitbox.Hitbox.Right - collider.GetHitbox().Left;
                    Position.X -= distance;
                }
                if (Velocity.X < 0)
                {
                    float distance = collider.GetHitbox().Right - Hitbox.Hitbox.Left;
                    Position.X += distance;
                }

                Velocity.X = 0;
                UpdateHitbox();
            }
        }
    }
    private void HandleVerticalCollision(List<ICollidable> colliders)
    {
        foreach (ICollidable collider in colliders)
        {
            if (SweptAABB(GetPreviousHitbox(), GetHitbox(), collider.GetHitbox()))
            {
                if (Velocity.Y > 0)
                {
                    float distance = Hitbox.Hitbox.Bottom - collider.GetHitbox().Top;
                    Position.Y -= distance;
                }
                if (Velocity.Y < 0)
                {
                    float distance = collider.GetHitbox().Bottom - Hitbox.Hitbox.Top;
                    Position.Y += distance;
                }

                Velocity.Y = 0;
                UpdateHitbox();
            }
        }
    }


    private bool CheckIfAirborne(List<ICollidable> colliders)
    {
        Rectangle hitboxForFloorCollision = GetHitbox();
        hitboxForFloorCollision.Offset(0, 1);
        
        foreach (ICollidable collider in colliders)
        {
            if (hitboxForFloorCollision.Intersects(collider.GetHitbox())) return false;
        }

        return true;
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


    public bool SweptAABB(Rectangle currentHitbox, Rectangle newHitbox, Rectangle obj)
    {
        if (_cheats.Noclip) return false;

        Rectangle union = Rectangle.Union(currentHitbox, newHitbox);
        return obj.Intersects(union);
    }
}