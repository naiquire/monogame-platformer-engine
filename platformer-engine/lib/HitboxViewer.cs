using System;
using System.ComponentModel;
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
        spriteBatch.Draw(_pixel, point, Color.Green);
    }
    public void DrawHitbox(SpriteBatch spriteBatch, Rectangle rectangle)
    {
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left,  rectangle.Top,    rectangle.Width, 1), Color.Red);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left,  rectangle.Bottom, rectangle.Width, 1), Color.Red);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left,  rectangle.Top,    1, rectangle.Height), Color.Red);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Right, rectangle.Top,    1, rectangle.Height + 1), Color.Red);
    }

    public void DrawHitbox(SpriteBatch spriteBatch, Circle circle)
    {
        const int segments = 64;
        float angleStep = MathF.Tau / segments;

        Point center = circle.Location;
        int radius = circle.Radius;

        Point point = center + new Point(radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;

            point = center + new Vector2(
                MathF.Cos(angle) * radius,
                MathF.Sin(angle) * radius
            ).ToPoint();

            spriteBatch.Draw(_pixel, point.ToVector2(), null, Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
        }
    }

}