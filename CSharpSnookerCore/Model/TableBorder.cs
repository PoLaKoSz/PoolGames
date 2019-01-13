using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CSharpSnookerCore.Model
{
    public class TableBorder
    {
        #region attributes
        int x = 0;
        int y = 0;
        int width = 0;
        int height = 0;
        ForcedDirection direction = ForcedDirection.None;
        public static string message;
        IBallObserver observer;
        #endregion attributes

        #region constructor
        public TableBorder(IBallObserver observer, int x, int y, int width, int height, ForcedDirection direction)
        {
            this.observer = observer;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.direction = direction;
        }
        #endregion constructor

        #region properties
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        #endregion properties

        #region functions
        public RectangleCollision Colliding(Ball ball)
        {
            RectangleCollision collision = RectangleCollision.None;

            if (!ball.IsBallInPocket)
            {
                if (x < 288 && (ball.X - Ball.Radius < x + width) && (ball.Y >= y && ball.Y <= y + height) && (ball.TranslateVelocity.X + ball.VSpinVelocity.X < 0.0d) && (ball.LastX > x + width))
                {
                    collision = RectangleCollision.Right;
                }
                else if (x > 288 && (ball.X + Ball.Radius > x) && (ball.Y >= y && ball.Y <= y + height) && (ball.TranslateVelocity.X + ball.VSpinVelocity.X > 0.0d) && (ball.LastX < x))
                {
                    collision = RectangleCollision.Left;
                }

                if (y < 161 && (ball.Y - Ball.Radius < y + height) && (ball.X >= x && ball.X - Ball.Radius <= x + width) && (ball.TranslateVelocity.Y + ball.VSpinVelocity.Y < 0.0d) && (ball.LastY > y) && (ball.LastX < x + width))
                {
                    collision = RectangleCollision.Bottom;
                }
                else if (y > 161 && (ball.Y + Ball.Radius > y) && (ball.X >= x && ball.X <= x + width) && (ball.TranslateVelocity.Y + ball.VSpinVelocity.Y > 0.0d) && (ball.LastY < y) && (ball.LastY < y) && (ball.LastX < x + width))
                {
                    collision = RectangleCollision.Top;
                }
            }

            return collision;
        }

        public void ResolveCollision(Ball ball, RectangleCollision collision)
        {
            double xSound = (float)(ball.Position.X - 300.0) / 300.0;
            observer.Hit(@"Sounds\Bank02.wav" + "|" + xSound.ToString());

            float absorption = 0.9f;

            if (this.direction == ForcedDirection.None)
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
                Vector2D position = new Vector2D(x + width / 2, y + height / 2);

                switch (this.direction)
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

                return;
                // get the mtd
                Vector2D delta = (position.Subtract(ball.Position));
                float d = delta.Lenght();
                // minimum translation distance to push balls apart after intersecting
                Vector2D mtd = delta.Multiply((float)(((this.width * 2) - d) / d));

                // resolve intersection --
                // inverse mass quantities
                float im1 = 0.5f;
                float im2 = 0.5f;

                // push-pull them apart based off their mass
                ball.Position = ball.Position.Subtract(mtd.Multiply(im2 / (im1 + im2)));

                // impact speed
                Vector2D v = ball.TranslateVelocity.Multiply(-1.0d);
                float vn = v.Dot(mtd.Normalize());
                //float vn = 1;

                // sphere intersecting but moving away from each other already
                if (vn > 0.0f)
                    return;

                // collision impulse
                float i = Math.Abs((float)((-(1.0f + 0.1) * vn) / (im1 + im2)));
                Vector2D impulse = mtd.Multiply(1);

                switch (this.direction)
                {
                    case ForcedDirection.Up:
                        ball.TranslateVelocity = ball.TranslateVelocity.Add(new Vector2D((double)0, (double)vn));
                        break;
                    case ForcedDirection.Down:
                        ball.TranslateVelocity = ball.TranslateVelocity.Subtract(new Vector2D((double)0, (double)vn));

                        break;
                    case ForcedDirection.Left:
                        ball.TranslateVelocity = ball.TranslateVelocity.Subtract(new Vector2D((double)vn, (double)0));
                        break;
                    case ForcedDirection.Right:
                        ball.TranslateVelocity = ball.TranslateVelocity.Add(new Vector2D((double)vn, (double)0));
                        break;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("TableBorder({0}, {1}, {2}, {3})", x, y, x + width, y + height);
        }
        #endregion functions
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
