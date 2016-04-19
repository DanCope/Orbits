using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Orbits.Domain;
using System;

namespace Orbits.Extensions
{
    public static class DrawBatchExtensions
    {
        public static void DrawConic(this DrawBatch drawBatch, Color color, Conic path, Vector2 offset, int scale = 0)
        {
            var angle = Math.Cos(path.Inclination) * path.ArgumentOfPeriapsis; //Might need to change
            var ellipseOffset = new Vector2((float)(path.FociToCenter * Math.Cos(angle)), (float)(path.FociToCenter * Math.Sin(angle)));
            var focus = (path.PrimeFocus.SystemPosition - ellipseOffset) / scale;

            drawBatch.DrawPrimitiveEllipse(new Pen(new SolidColorBrush(color)),
                focus + offset,
                (float)path.SemiMajorAxis / scale,
                (float)path.SemiMinorAxis / scale,
                (float)angle,
                1024);
            
        }

        public static void DrawBody(this DrawBatch drawBatch, Color color, Body body, Vector2 offset, int scale = 0)
        {
            var drawPosition = (body.SystemPosition / scale) + offset;
            var drawRadius = body.Radius / scale;
            var drawEdgePosition = new Vector2(
                (float)(drawPosition.X + drawRadius * Math.Cos(body.Rotation)),
                (float)(drawPosition.Y + drawRadius * Math.Sin(body.Rotation)));

            // Check body is actually on screen
            if (drawPosition.X < 0 || drawPosition.X > 1920) return;
            if (drawPosition.Y < 0 || drawPosition.Y > 1080) return;

            drawBatch.FillCircle(new SolidColorBrush(color), drawPosition, (float)drawRadius);
            drawBatch.DrawPrimitiveLine(new Pen(new SolidColorBrush(Color.HotPink)), drawPosition, drawEdgePosition);
        }
         
        public static void DrawCursor(this DrawBatch drawBatch)
        {
            var mouseState = Mouse.GetState();
            var col = Color.White;
            if (mouseState.LeftButton == ButtonState.Pressed)
                col = Color.Red;
            else if (mouseState.RightButton == ButtonState.Pressed)
                col = Color.Orange;

            drawBatch.FillCircle(new SolidColorBrush(col), new Vector2(mouseState.X, mouseState.Y), 2);
        }
    }
}
