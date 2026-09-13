using System;
using lib;
using lib.Structures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    public void DrawHitbox(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        rectangle.Offset(Core.Camera.GetDrawingOffset());
        if (!CameraManager.IsVisible(rectangle)) return;

        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left,  rectangle.Top,    rectangle.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left,  rectangle.Bottom, rectangle.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left,  rectangle.Top,    1, rectangle.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Right, rectangle.Top,    1, rectangle.Height + 1), color);
    }

    public void DrawHitbox(SpriteBatch spriteBatch, Circle circle)
    {
        const int segments = 64;
        float angleStep = MathF.Tau / segments;

        Point center = circle.Location + Core.Camera.GetDrawingOffset().ToPoint();
        int radius = circle.Radius;

        if (!CameraManager.IsVisible((center - new Point(radius, radius)).ToVector2(), new Vector2(radius, radius) * 2)) return;

        Point point;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;

            point = center + new Vector2(
                MathF.Cos(angle) * radius,
                MathF.Sin(angle) * radius
            ).ToPoint();

            spriteBatch.Draw(_pixel, point.ToVector2(), Color.Red);
        }
    }

    public void DrawHitbox(SpriteBatch spriteBatch, RightTriangle triangle, Color color)
    {
        Vector2 rightVertex = triangle.Location.ToVector2();
        rightVertex += Core.Camera.GetDrawingOffset();

        spriteBatch.Draw(_pixel, new Rectangle(triangle.Left, triangle.Bottom, triangle.Width, 1), color);
        spriteBatch.Draw(_pixel, new Rectangle(triangle.Right, triangle.Top, 1, triangle.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle(triangle.Left, triangle.Bottom, (int)triangle.HypotenuseLength, 1), null, color, MathF.Atan2(triangle.Height, triangle.Width), new Vector2(triangle.Left, triangle.Bottom), SpriteEffects.None, 0);
    }
}