using System;
using System.ComponentModel;
using lib.Colliders;
using lib.Input;
using lib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using platformer_engine;

namespace Entities;

enum Direction
{
    Up, Down, Left, Right
}
enum DashState
{
    None, Dashing
}
enum ClingState
{
    None, Left, Right
}
enum CrouchState
{
    None, Crouching
}
enum PlayerAction
{
    Up, Down, Left, Right, Jump, Dash
}

public class Player(LevelScene scene, Vector2 position) : Entity(scene, position)
{
    private struct PlayerInfo
    {
        public DashInfo Dash;
        public ClingInfo Cling;
        public CrouchState CrouchState;
        public bool IsAirborne;
        public Direction DirectionFacing;
    }
    private struct DashInfo
    {
        public DashState DashState;
        public bool IsDashAvailable;
        public double DashTimeRemaining;
        public DashInfo()
        {
            DashState = DashState.None;
            IsDashAvailable = false;
            DashTimeRemaining = -1;
        }
    }
    private struct ClingInfo
    {
        public ClingState ClingState;
        public double WallJumpTimeRemaining;
        public ClingInfo()
        {
            ClingState = ClingState.None;
            WallJumpTimeRemaining = -1;
        }
        public readonly Direction InvertDirection()
        {
            if (ClingState == ClingState.Left)
            {
                return Direction.Left;
            }
            if (ClingState == ClingState.Right)
            {
                return Direction.Right;
            }

            throw new InvalidEnumArgumentException("Player must be clinged to a wall.");
        }
    }

    private readonly int _playerIndex;
    private Vector2 _playerSize;
    private PlayerInfo _playerState;
    private InputController<PlayerAction> Input;

    public void Initialize()
    {
        _playerState = new()
        {
            Dash = new(),
            Cling = new(),
            CrouchState = CrouchState.None,
            IsAirborne = true,
            DirectionFacing = Direction.Right
        };

        _playerSize = new(40, 80);
        InitializeKeybindings();
        GenerateHitbox((int)_playerSize.X, (int)_playerSize.Y, Alignment.Bottom);
    }
    private void InitializeKeybindings()
    {
        Input = new InputController<PlayerAction>();

        Input.SetBinding(PlayerAction.Up, new ActionBindings([Keys.Up], [], []));
        Input.SetBinding(PlayerAction.Down, new ActionBindings([Keys.Down], [], []));
        Input.SetBinding(PlayerAction.Left, new ActionBindings([Keys.Left], [], []));
        Input.SetBinding(PlayerAction.Right, new ActionBindings([Keys.Right], [], []));

        Input.SetBinding(PlayerAction.Dash, new ActionBindings([Keys.C], [], []));
        Input.SetBinding(PlayerAction.Jump, new ActionBindings([Keys.Z], [], []));
    }

    private void UpdateState()
    {
        _playerState.IsAirborne = IsPlayerAirborne();
        
        bool clingedLeft = CheckIfClingedLeft();
        bool clingedRight = CheckIfClingedRight();

        _playerState.Cling.ClingState = ClingState.None;
        if (clingedLeft && _playerState.IsAirborne) _playerState.Cling.ClingState = ClingState.Left;
        if (clingedRight && _playerState.IsAirborne) _playerState.Cling.ClingState = ClingState.Right;

        if (!_playerState.IsAirborne || _playerState.Cling.ClingState != ClingState.None)
        {
            _playerState.Dash.IsDashAvailable = true;
        }

        Console.WriteLine($"Dash:{_playerState.Dash.DashState},{_playerState.Dash.IsDashAvailable && _playerState.Dash.DashTimeRemaining == -1} Dir:{_playerState.DirectionFacing} Cling:{_playerState.Cling.ClingState} Fly:{_playerState.IsAirborne}");
    }
    public override void Update(GameTime gameTime)
    {
        bool crouchInputted = Input.IsActionPressed(PlayerAction.Down);
        if (crouchInputted && !_playerState.IsAirborne) Crouch();

        bool uncrouchInputted = Input.IsActionReleased(PlayerAction.Down);
        if (uncrouchInputted || _playerState.IsAirborne && _playerState.Dash.DashState == DashState.None) Uncrouch();

        bool dashInputted = Input.WasActionJustPressed(PlayerAction.Dash);
        if (dashInputted) RequestDash(crouchInputted);

        switch (_playerState.Dash.DashState)
        {
            case DashState.None:
                Position.X = UpdateHorizontalPosition(gameTime);
                UpdateHitbox(Position);

                Position.Y = UpdateVerticalPosition(gameTime);
                UpdateHitbox(Position);

                UpdateWallJump(gameTime);

                break;
            case DashState.Dashing:
                Position.X = UpdateDash(gameTime);
                Velocity.Y = 0;
                UpdateHitbox(Position);
                
                break;
        }

        UpdateState();
        base.Update(gameTime);
    }

    /// <summary>
    /// Calculates the new horizontal position of the player, and updates the direction which the player is facing.
    /// </summary>
    /// <returns>The new horizontal position of the player.</returns>
    private float UpdateHorizontalPosition(GameTime gameTime)
    {
        float horizontalPosition;

        Velocity.X = UpdateHorizontalVelocity(gameTime);
        if (Velocity.X > 0)
        {
            _playerState.DirectionFacing = Direction.Right;
        }
        if (Velocity.X < 0)
        {
            _playerState.DirectionFacing = Direction.Left;
        }

        horizontalPosition = Position.X + Velocity.X;
        UpdateHitbox(horizontalPosition, Position.Y);
        float delta = HandleHorizontalCollision();

        return Position.X + Velocity.X + delta;        
    }

    /// <summary>
    /// Calculates the new horizontal velocity of the player based on user input.
    /// </summary>
    /// <returns>The new horizontal velocity of the player.</returns>
    private float UpdateHorizontalVelocity(GameTime gameTime)
    {
        const float Xspeed = 10f;

        if (_playerState.Cling.WallJumpTimeRemaining != -1)
        {
            if (_playerState.DirectionFacing == Direction.Left) return -Xspeed;
            if (_playerState.DirectionFacing == Direction.Right) return Xspeed;
            return 0;
        }

        bool leftInputted = Input.IsActionPressed(PlayerAction.Left);
        bool rightInputted = Input.IsActionPressed(PlayerAction.Right);

        if (leftInputted && !rightInputted)
        {
            return -Xspeed;
        }
        if (!leftInputted && rightInputted)
        {
            return Xspeed;
        }

        return 0;
    }

    /// <summary>
    /// Calculates the new vertical position of the player.
    /// </summary>
    /// <returns>The new vertical position of the player.</returns>
    private float UpdateVerticalPosition(GameTime gameTime)
    {
        float verticalPosition;

        Velocity.Y = UpdateVerticalVelocity(gameTime);

        verticalPosition = Position.Y + Velocity.Y;
        UpdateHitbox(Position.X, verticalPosition);
        float delta = HandleVerticalCollision();

        float returnValue = Position.Y + Velocity.Y + delta;
        if (delta != 0) Velocity.Y = 0;
        return returnValue;
    }

    /// <summary>
    /// Calculates the new vertical velocity of the player based on user input.
    /// </summary>
    /// <returns>The new vertical velocity of the player.</returns>
    private float UpdateVerticalVelocity(GameTime gameTime)
    {
        const float gravity = 0.5f;
        float verticalVelocity = 0;

        bool jumpInputted = Input.WasActionJustPressed(PlayerAction.Jump);
        if (jumpInputted)
        {
            if (RequestJump())
            {
                return -10f;
            }
        }
        
        if (_playerState.IsAirborne)
        {
            verticalVelocity = Velocity.Y + gravity;
        }

        if (_playerState.Cling.ClingState == ClingState.None)
        {
            verticalVelocity = Math.Min(verticalVelocity, 20);
        }
        else
        {
            verticalVelocity = Math.Min(verticalVelocity, 5);
        }

        return verticalVelocity;
    }
    
    private float HandleHorizontalCollision()
    {
        float horizontalDelta = 0;

        foreach (ICollidable collider in Scene.Objects)
        {
            Rectangle colliderHitbox = collider.GetHitbox();
            if (SweptAABB(GetPreviousHitbox(), GetHitbox(), colliderHitbox))
            {
                if (_playerState.DirectionFacing == Direction.Right)
                {
                    float distance = Hitbox.Hitbox.Right - colliderHitbox.Left;
                    horizontalDelta = Math.Min(-distance, horizontalDelta);
                }
                if (_playerState.DirectionFacing == Direction.Left)
                {
                    float distance = colliderHitbox.Right - Hitbox.Hitbox.Left;
                    horizontalDelta = Math.Max(distance, horizontalDelta);
                }
            }
        }

        return horizontalDelta;
    }
    private float HandleVerticalCollision()
    {
        float verticalDelta = 0;

        foreach (ICollidable collider in Scene.Objects)
        {
            Rectangle colliderHitbox = collider.GetHitbox();
            if (SweptAABB(GetPreviousHitbox(), GetHitbox(), colliderHitbox))
            {
                if (Velocity.Y > 0)
                {
                    float distance = Hitbox.Hitbox.Bottom - colliderHitbox.Top;
                    verticalDelta = Math.Min(-distance, verticalDelta);
                }
                if (Velocity.Y < 0)
                {
                    float distance = colliderHitbox.Bottom - Hitbox.Hitbox.Top;
                    verticalDelta = Math.Max(distance, verticalDelta);
                }
            }
        }

        return verticalDelta;
    }

    private bool IsPlayerAirborne()
    {
        Rectangle hitboxForFloorCollision = GetHitbox();
        hitboxForFloorCollision.Offset(0, 1);
        
        foreach (ICollidable collider in Scene.Objects)
        {
            if (hitboxForFloorCollision.Intersects(collider.GetHitbox())) return false;
        }

        return true;
    }
    private bool CheckIfClingedLeft()
    {
        if (_playerState.DirectionFacing == Direction.Left) return false;
        if (Input.IsActionReleased(PlayerAction.Right) && _playerState.Dash.DashState == DashState.None && _playerState.Cling.ClingState == ClingState.None) return false;

        Rectangle leftWall = GetHitbox();
        leftWall.Offset(1, 0);

        foreach (ICollidable collider in Scene.Objects)
        {
            if (leftWall.Intersects(collider.GetHitbox())) return true;
        }

        return false;
    }
    private bool CheckIfClingedRight()
    {
        if (_playerState.DirectionFacing == Direction.Right) return false;
        if (Input.IsActionReleased(PlayerAction.Left) && _playerState.Dash.DashState == DashState.None && _playerState.Cling.ClingState == ClingState.None) return false;

        Rectangle rightWall = GetHitbox();
        rightWall.Offset(-1, 0);

        foreach (ICollidable collider in Scene.Objects)
        {
            if (rightWall.Intersects(collider.GetHitbox())) return true;
        }

        return false;
    }

    private bool RequestJump()
    {
        if (!_playerState.IsAirborne)
        {
            return true;
        }
        if (_playerState.Cling.ClingState != ClingState.None)
        {
            _playerState.Cling.WallJumpTimeRemaining = 0.1f;
            _playerState.DirectionFacing = _playerState.Cling.InvertDirection();

            return true;
        }

        return false;
    }
    private bool RequestDash(bool crouched)
    {
        if (!(_playerState.Dash.IsDashAvailable && _playerState.Dash.DashTimeRemaining == -1))
        {
            return false;
        }

        if (crouched) Crouch();

        if (_playerState.Cling.ClingState != ClingState.None)
        {
            _playerState.DirectionFacing = _playerState.Cling.InvertDirection();
        }

        if (_playerState.IsAirborne)
        {
            _playerState.Dash.IsDashAvailable = false;
        }
        _playerState.Dash.DashState = DashState.Dashing;
        _playerState.Dash.DashTimeRemaining = 0.1f;

        return true;
    }
    private float UpdateDash(GameTime gameTime)
    {
        const float dashSpeed = 30f;
        float horizontalPosition;

        if (_playerState.DirectionFacing == Direction.Left)
        {
            Velocity.X = -dashSpeed;
        }
        if (_playerState.DirectionFacing == Direction.Right)
        {
            Velocity.X = dashSpeed;
        }
        
        _playerState.Dash.DashTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
        if (_playerState.Dash.DashTimeRemaining <= 0)
        {
            _playerState.Dash.DashState = DashState.None;
            _playerState.Dash.DashTimeRemaining = -1;
            if (_playerState.IsAirborne) Uncrouch();
        }

        horizontalPosition = Position.X + Velocity.X;
        UpdateHitbox(horizontalPosition, Position.Y);
        float delta = HandleHorizontalCollision();

        return Position.X + Velocity.X + delta;  
    }

    private void Crouch()
    {
        if (_playerState.CrouchState == CrouchState.Crouching) return;

        _playerState.CrouchState = CrouchState.Crouching;
        GenerateHitbox((int)_playerSize.X, (int)_playerSize.Y / 2, Hitbox.Alignment);
    }
    private void Uncrouch()
    {
        if (_playerState.CrouchState == CrouchState.None) return;

        _playerState.CrouchState = CrouchState.None;
        GenerateHitbox((int)_playerSize.X, (int)_playerSize.Y, Hitbox.Alignment);

        foreach (ICollidable collider in Scene.Objects)
        {
            if (AABB(GetHitbox(), collider.GetHitbox()))
            {
                Crouch();
                return;
            }
        }
    }

    private void UpdateWallJump(GameTime gameTime)
    {
        if (_playerState.Cling.WallJumpTimeRemaining != -1)
        {
            _playerState.Cling.WallJumpTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_playerState.Cling.WallJumpTimeRemaining < 0)
            {
                _playerState.Cling.WallJumpTimeRemaining = -1;
            }
        }
    }
}
