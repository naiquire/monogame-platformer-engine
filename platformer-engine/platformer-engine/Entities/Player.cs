using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using lib;
using lib.Colliders.Entities;
using lib.Colliders;
using lib.Scenes;
using System;

namespace Entities;

public enum Direction
{
    Up, Down, Left, Right
}
public enum State
{
    Normal, Dashing
}
public enum ClingState
{
    None, Left, Right
}

public record PlayerState
{
    public State State;
    public ClingState ClingState;
    public Direction DirectionFacing;
    public double DashTimeRemaining;
    public double WallJumpTimeRemaining;
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
            CanDash = false,
            DashTimeRemaining = -1,
            IsCrouched = false,
            ClingState = ClingState.None
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
        switch (_playerState.State)
        {
            case State.Normal:

                UpdateXPosition(gameTime, scene);
                UpdateYPosition(gameTime, scene);

                break;
            case State.Dashing:

                UpdateXPosition(gameTime, scene);
                
                break;
            default:
                break;
        }

        CheckKeystrokes(scene);
        UpdateStates(scene);
        Console.WriteLine($"{_playerState.State} , {_playerState.DirectionFacing} , {_playerState.ClingState}");

        base.Update(gameTime, scene);
    }
    private void UpdateStates(Scene scene)
    {
        // update state values
        _playerState.IsAirborne = IsPlayerAirborne(scene.LevelObjects);
        if (CanDashBeReplenished()) _playerState.CanDash = true;

        _playerState.ClingState = ClingState.None;
        bool clingedLeft = CheckIfClingedLeft(scene.LevelObjects);
        bool clingedRight = CheckIfClingedRight(scene.LevelObjects);

        if (clingedLeft && _playerState.IsAirborne) _playerState.ClingState = ClingState.Left;
        if (clingedRight && _playerState.IsAirborne) _playerState.ClingState = ClingState.Right;
    }
    private void CheckKeystrokes(Scene scene)
    {
        // dashing
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.C))
        {
            if (_playerState.DashTimeRemaining == -1 && (_playerState.CanDash || _cheats.InfiniteDash))
            {
                Dash();
            }
        }

        // crouch
        if (Core.Input.Keyboard.IsKeyDown(Keys.Down) && !_playerState.IsAirborne)
        {
            Crouch();
        }
        if (Core.Input.Keyboard.IsKeyUp(Keys.Down) && _playerState.IsCrouched)
        {
            AttemptUncrouch(scene);
        }
    }

    private void UpdateXPosition(GameTime gameTime, Scene scene)
    {
        switch (_playerState.State)
        {
            case State.Normal:
                UpdateXVelocity(gameTime);
                break;
            case State.Dashing:
                UpdateDash(gameTime);
                break;
        }

        Position.X += Velocity.X;
        UpdateHitbox();
        HandleHorizontalCollision(scene.LevelObjects);
    }
    private void UpdateYPosition(GameTime gameTime, Scene scene)
    {
        UpdateYVelocity(scene);
        Position.Y += Velocity.Y;
        UpdateHitbox();
        HandleVerticalCollision(scene.LevelObjects);
    }

    private void UpdateXVelocity(GameTime gameTime)
    {
        if (_playerState.WallJumpTimeRemaining > 0)
        {
            UpdateWallJump(gameTime);
            return;
        }

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
    private void UpdateYVelocity(Scene scene)
    {
        // jumping
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Z))
        {            
            if (!_playerState.IsAirborne || _cheats.InfiniteJump)
            {
                Jump(scene);
            }
            else if (_playerState.ClingState != ClingState.None)
            {
                WallJump(scene);
            }
        }

        // handle gravity
        if (_playerState.IsAirborne) Velocity += _gravity;
        if (Velocity.Y > 20)
        {
            Velocity.Y = 20;
        }
    }

    private void Jump(Scene scene)
    {
        if (_playerState.IsCrouched)
        {
            AttemptUncrouch(scene);
        }

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

        Velocity.Y = -y_speed;
        _playerState.IsAirborne = true;

        
    }
    private void WallJump(Scene scene)
    {
        if (_playerState.ClingState == ClingState.Left)
        {
            _playerState.DirectionFacing = Direction.Left;
        }
        if (_playerState.ClingState == ClingState.Right)
        {
            _playerState.DirectionFacing = Direction.Right;
        }

        Jump(scene);
        _playerState.WallJumpTimeRemaining = 0.1f;
    }
    private void UpdateWallJump(GameTime gameTime)
    {
        const float x_speed = 10;

        if (_playerState.DirectionFacing == Direction.Left)
        {
            Velocity.X = -x_speed;
        }
        if (_playerState.DirectionFacing == Direction.Right)
        {
            Velocity.X = +x_speed;
        }
        
        _playerState.WallJumpTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;

        if (_playerState.WallJumpTimeRemaining <= 0)
        {
            _playerState.WallJumpTimeRemaining = -1;
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

    private void Dash()
    {
        if (_playerState.ClingState == ClingState.Left)
        {
            _playerState.DirectionFacing = Direction.Left;
        }
        if (_playerState.ClingState == ClingState.Right)
        {
            _playerState.DirectionFacing = Direction.Right;
        }

        _playerState.State = State.Dashing;
        _playerState.DashTimeRemaining = 0.1f;
        _playerState.CanDash = false;
    }
    private void UpdateDash(GameTime gameTime)
    {
        const float dashSpeed = 30f;

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
        }
    }
    private bool CanDashBeReplenished()
    {
        return !_playerState.IsAirborne || (_playerState.State != State.Dashing && _playerState.ClingState != ClingState.None);
    }

    private bool IsPlayerAirborne(List<ICollidable> colliders)
    {
        Rectangle hitboxForFloorCollision = GetHitbox();
        hitboxForFloorCollision.Offset(0, 1);
        
        foreach (ICollidable collider in colliders)
        {
            if (hitboxForFloorCollision.Intersects(collider.GetHitbox())) return false;
        }

        return true;
    }
    private bool CheckIfClingedLeft(List<ICollidable> colliders)
    {
        Rectangle leftWall = GetHitbox();
        leftWall.Offset(1, 0);

        foreach (ICollidable collider in colliders)
        {
            if (leftWall.Intersects(collider.GetHitbox())) return true;
        }

        return false;
    }
    private bool CheckIfClingedRight(List<ICollidable> colliders)
    {
        Rectangle rightWall = GetHitbox();
        rightWall.Offset(-1, 0);

        foreach (ICollidable collider in colliders)
        {
            if (rightWall.Intersects(collider.GetHitbox())) return true;
        }

        return false;
    }

    private void Crouch()
    {
        _playerState.IsCrouched = true;
        GenerateHitbox(30, 30, Hitbox.Alignment);
    }

    private bool AttemptUncrouch(Scene scene)
    {
        Uncrouch();

        foreach (ICollidable collider in scene.LevelObjects)
        {
            if (AABB(GetHitbox(), collider.GetHitbox()))
            {
                Crouch();
                return false;
            }
        }

        return true;
    }
    private void Uncrouch()
    {
        _playerState.IsCrouched = false;
        GenerateHitbox(30, 60, Hitbox.Alignment);
    }

    public bool AABB(Rectangle hitbox, Rectangle obj)
    {
        return obj.Intersects(hitbox);
    }
    public bool SweptAABB(Rectangle currentHitbox, Rectangle newHitbox, Rectangle obj)
    {
        if (_cheats.Noclip) return false;

        Rectangle union = Rectangle.Union(currentHitbox, newHitbox);
        return obj.Intersects(union);
    }
}
