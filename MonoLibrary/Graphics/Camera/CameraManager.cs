using MonoLibrary;
using MonoLibrary.Colliders;
using Microsoft.Xna.Framework;

namespace MonoLibrary.Graphics.Camera;
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
    /// The position of the camera with respect to the origin of the screen.
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// A rectangle which defines the coordinates that are currently within the camera's view.
    /// </summary>
    public static Quadrilateral ScreenBounds { get; private set; }

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
    public void Update(Polygon focusPoint)
    {
        switch (CameraMode)
        {
            case CameraMode.Static:
                break;
            case CameraMode.Standard:

                const float cutoffPoint = 0.25f;

                float leftBound = cutoffPoint * Core.GetScreenDimensions().X;
                float rightBound = (1 - cutoffPoint) * Core.GetScreenDimensions().X;
                float upperBound = cutoffPoint * Core.GetScreenDimensions().Y;
                float lowerBound = (1 - cutoffPoint) * Core.GetScreenDimensions().Y;

                if (focusPoint.Position.X < Position.X + leftBound)
                {
                    SetPosition(new(focusPoint.Position.X - leftBound, Position.Y));
                }
                if (focusPoint.Position.X > Position.X + rightBound)
                {
                    SetPosition(new(focusPoint.Position.X - rightBound, Position.Y));
                }
                if (focusPoint.Position.Y < Position.Y + upperBound)
                {
                    SetPosition(new(Position.X, focusPoint.Position.Y - upperBound));
                }
                if (focusPoint.Position.Y > Position.Y + lowerBound)
                {
                    SetPosition(new(Position.X, focusPoint.Position.Y - lowerBound));
                }

                break;
            case CameraMode.Follow:
                Position = focusPoint.Position - Core.GetScreenDimensions() * 0.5f;
                break;
        }

        ScreenBounds = Quadrilateral.FromRectangle(Core.GraphicsDevice.PresentationParameters.Bounds);
    }

    /// <summary>
    /// Calculates a vector to render sprites at the correct location.
    /// </summary>
    /// <returns>A vector which will draw the sprites correctly when added to their position.</returns>
    public Vector2 GetDrawingOffset()
    {
        return -Position;
    }

    /// <summary>
    /// Determines if an object is visible on the screen based on the camera.
    /// </summary>
    /// <returns>A boolean value representing whether the object is visible or not.</returns>
    public static bool IsVisible(Collider<Polygon> obj)
    {
        return IsVisible(obj.Hitbox);
    }

    /// <summary>
    /// Determines if an object is visible on the screen based on the camera.
    /// </summary>
    /// <returns>A boolean value representing whether the object is visible or not.</returns>
    public static bool IsVisible(Polygon obj)
    {
        return obj.Intersects(ScreenBounds);
    }

    /// <summary>
    /// Determines if an object is visible on the screen based on the camera.
    /// </summary>
    /// <returns>A boolean value representing whether the object is visible or not.</returns>
    public static bool IsVisible(Vector2 position, Vector2 boundingSize)
    {
        return IsVisible(new Quadrilateral(position, boundingSize));
    }
}