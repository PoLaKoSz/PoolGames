using System;

namespace CSharpSnookerCore.Models
{
    public class TableBorder
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }


        ForcedDirection direction = ForcedDirection.None;
        public static string message;
        private readonly IBorderObserver _observer;



        public TableBorder(IBorderObserver observer, int x, int y, int width, int height, ForcedDirection direction)
        {
            this._observer = observer;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            this.direction = direction;
        }





        public RectangleCollision Colliding(Ball ball)
        {
            RectangleCollision collision = RectangleCollision.None;

            if (!ball.IsBallInPocket)
            {
                if (X < 288 && (ball.X - Ball.Radius < X + Width) && (ball.Y >= Y && ball.Y <= Y + Height) && (ball.TranslateVelocity.X + ball.VSpinVelocity.X < 0.0d) && (ball.LastX > X + Width))
                {
                    collision = RectangleCollision.Right;
                }
                else if (X > 288 && (ball.X + Ball.Radius > X) && (ball.Y >= Y && ball.Y <= Y + Height) && (ball.TranslateVelocity.X + ball.VSpinVelocity.X > 0.0d) && (ball.LastX < X))
                {
                    collision = RectangleCollision.Left;
                }

                if (Y < 161 && (ball.Y - Ball.Radius < Y + Height) && (ball.X >= X && ball.X - Ball.Radius <= X + Width) && (ball.TranslateVelocity.Y + ball.VSpinVelocity.Y < 0.0d) && (ball.LastY > Y) && (ball.LastX < X + Width))
                {
                    collision = RectangleCollision.Bottom;
                }
                else if (Y > 161 && (ball.Y + Ball.Radius > Y) && (ball.X >= X && ball.X <= X + Width) && (ball.TranslateVelocity.Y + ball.VSpinVelocity.Y > 0.0d) && (ball.LastY < Y) && (ball.LastY < Y) && (ball.LastX < X + Width))
                {
                    collision = RectangleCollision.Top;
                }
            }

            return collision;
        }

        public void ResolveCollision(Ball ball, RectangleCollision collision)
        {
            _observer.WallCollision(ball);

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
                Vector2D position = new Vector2D(X + Width / 2, Y + Height / 2);

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
                Vector2D mtd = delta.Multiply((float)(((this.Width * 2) - d) / d));

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
