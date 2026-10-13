using System;
using MonoLibrary.Graphics.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoLibrary.Structures;

namespace MonoLibrary;
public class HitboxViewer
{
    private readonly Texture2D _pixel;

    public HitboxViewer(GraphicsDevice graphicsDevice)
    {
        _pixel = new(graphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);
    }

    public void DrawPoint(SpriteBatch spriteBatch, Vector2 point)
    {
        point = Core.Camera.ApplyDrawingOffset(point);
        if (!CameraManager.IsVisible(point, new Vector2(1, 1) * Core.Camera.Zoom)) return;

        spriteBatch.Draw(_pixel, point, Color.White);
    }
    public void DrawHitbox(SpriteBatch spriteBatch, Quadrilateral rectangle, Color color)
    {
        Quadrilateral box = new(Core.Camera.ApplyDrawingOffset(rectangle.Position), rectangle.BoundingSize * Core.Camera.Zoom);
        if (!CameraManager.IsVisible(rectangle)) return;

        spriteBatch.Draw(_pixel, new Rectangle((int)box.Left,  (int)box.Top,    (int)box.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)box.Left,  (int)box.Bottom, (int)box.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)box.Left,  (int)box.Top,    1, (int)box.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)box.Right, (int)box.Top,    1, (int)box.Height), color);
    }

    public void DrawHitbox(SpriteBatch spriteBatch, Circle circle)
    {
        const int segments = 64;
        float angleStep = MathF.Tau / segments;

        Vector2 center = Core.Camera.ApplyDrawingOffset(circle.Center);
        float radius = circle.Radius * Core.Camera.Zoom;

        if (!CameraManager.IsVisible(circle)) return;

        Vector2 point;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;

            point = center + new Vector2(
                MathF.Cos(angle) * radius,
                MathF.Sin(angle) * radius
            );

            spriteBatch.Draw(_pixel, point, Color.Red);
        }
    }

    public void DrawHitbox(SpriteBatch spriteBatch, RightTriangle triangle, Color color)
    {
        triangle = new(Core.Camera.ApplyDrawingOffset(triangle.RightAngleVertex), triangle.BoundingSize * Core.Camera.Zoom, triangle.Direction);
        if (!CameraManager.IsVisible(triangle)) return;

        spriteBatch.Draw(_pixel, new Rectangle((int)triangle.Left, (int)triangle.Bottom, (int)triangle.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)triangle.Right, (int)triangle.Top, 1, (int)triangle.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)triangle.Left, (int)triangle.Bottom, (int)triangle.HypotenuseLength, 1), null, color, -MathF.Atan2(triangle.Height, triangle.Width), new Vector2(0, 0.5f), SpriteEffects.None, 0);
    }
}