using System;

namespace CSharpSnookerCore.Models
{
    public class TableBorder
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }


        private readonly ForcedDirection Direction;



        public TableBorder(int x, int y, int width, int height, ForcedDirection direction)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Direction = direction;
        }



        public void ResolveCollision(Ball ball, RectangleCollision collision)
        {
            float absorption = 0.9f;

            if (this.Direction == ForcedDirection.None)
            {
                switch (collision)
                {
                    case RectangleCollision.Right:
                    case RectangleCollision.Left:
                        if (Math.Sign(ball.TranslateVelocity.X) == Math.Sign(ball.VSpinVelocity.X) && ball.VSpinVelocity.X > 0.0)
                        {
                            ball.TranslateVelocity.X += ball.VSpinVelocity.X;
                            ball.VSpinVelocity.X = (double)0.0;
                        }
                        ball.TranslateVelocity.X *= -1.0d * absorption;
                        break;
                    case RectangleCollision.Bottom:
                    case RectangleCollision.Top:
                        if (Math.Sign(ball.TranslateVelocity.Y) == Math.Sign(ball.VSpinVelocity.Y) && ball.VSpinVelocity.Y > 0.0)
                        {
                            ball.TranslateVelocity.Y += ball.VSpinVelocity.Y;
                            ball.VSpinVelocity.Y = (double)0.0;
                        }
                        ball.TranslateVelocity.Y *= -1.0d * absorption;
                        break;
                }
            }
            else
            {
                Vector2D position = new Vector2D(X + Width / 2, Y + Height / 2);

                switch (this.Direction)
                {
                    case ForcedDirection.Up:
                        ball.TranslateVelocity.Y *= -0.5d;
                        ball.TranslateVelocity.X = ball.TranslateVelocity.Y * -0.5d;
                        break;
                    case ForcedDirection.Down:
                        ball.TranslateVelocity.Y *= -0.5d;
                        ball.TranslateVelocity.X = ball.TranslateVelocity.Y * -0.5d;
                        break;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("TableBorder({0}, {1}, {2}, {3})", X, Y, X + Width, Y + Height);
        }
    }

    public enum RectangleCollision
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public enum ForcedDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
}
