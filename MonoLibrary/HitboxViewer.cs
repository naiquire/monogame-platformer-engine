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
        point += Core.Camera.GetDrawingOffset();
        if (!CameraManager.IsVisible(point, new(1, 1))) return;

        spriteBatch.Draw(_pixel, point, Color.Black);
    }
    public void DrawHitbox(SpriteBatch spriteBatch, Quadrilateral rectangle, Color color)
    {
        rectangle.Offset(Core.Camera.GetDrawingOffset());
        //if (!CameraManager.IsVisible(rectangle)) return;

        spriteBatch.Draw(_pixel, new Rectangle((int)rectangle.Left,  (int)rectangle.Top,    (int)rectangle.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)rectangle.Left,  (int)rectangle.Bottom, (int)rectangle.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)rectangle.Left,  (int)rectangle.Top,    1, (int)rectangle.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)rectangle.Right, (int)rectangle.Top,    1, (int)rectangle.Height), color);
    }

    public void DrawHitbox(SpriteBatch spriteBatch, Circle circle)
    {
        const int segments = 64;
        float angleStep = MathF.Tau / segments;

        Vector2 center = circle.Center + Core.Camera.GetDrawingOffset();
        float radius = circle.Radius;

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
        triangle = new(triangle.RightAngleVertex + Core.Camera.GetDrawingOffset(), triangle.BoundingSize, triangle.Direction);
        if (!CameraManager.IsVisible(triangle)) return;

        spriteBatch.Draw(_pixel, new Rectangle((int)triangle.Left, (int)triangle.Bottom, (int)triangle.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)triangle.Right, (int)triangle.Top, 1, (int)triangle.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle((int)triangle.Left, (int)triangle.Bottom, (int)triangle.HypotenuseLength, 1), null, color, -MathF.Atan2(triangle.Height, triangle.Width), new Vector2(0, 0.5f), SpriteEffects.None, 0);
    }
}