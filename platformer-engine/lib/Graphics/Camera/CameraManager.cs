using System.Collections;
using System.Reflection.Metadata.Ecma335;
using lib;
using lib.Colliders;
using Microsoft.Xna.Framework;

public enum CameraMode
{
    /// <summary>
    /// The camera position will not be updated.
    /// </summary>
    Static,
    /// <summary>
    /// The camera position will smoothly follow the player
    /// </summary>
    Standard,
    /// <summary>
    /// The camera position will force the player to always be centered.
    /// </summary>
    Follow
}
public class CameraManager
{
    /// <summary>
    /// The position of the camera with respect to the origin of the screen. The position is 
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// The current tracking mode which the camera is in.
    /// </summary>
    public CameraMode CameraMode { get; set; } = CameraMode.Static;

    /// <summary>
    /// Immediately forces the camera to a given position.
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(Vector2 position)
    {
        Position = position;
    }

    /// <summary>
    /// Updates the position of the camera to follow the given entity.
    /// </summary>
    /// <param name="focusPoint">The entity to position the camera on.</param>
    public void Update(Collider focusPoint)
    {
        switch (CameraMode)
        {
            case CameraMode.Static:
                break;
            case CameraMode.Standard:
                break;
            case CameraMode.Follow:
                Position = focusPoint.Position - new Vector2(Core.GraphicsDevice.PresentationParameters.BackBufferWidth, Core.GraphicsDevice.PresentationParameters.BackBufferHeight) / 2;
                break;
        }
    }

    /// <summary>
    /// Calculates a vector to render sprites at the correct location.
    /// </summary>
    /// <returns>A vector which will draw the sprites correctly when added to their position.</returns>
    public Vector2 GetDrawingOffset()
    {
        return Position * -1;
    }
}