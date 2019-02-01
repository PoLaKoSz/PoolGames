using CSharpSnooker.WinForms.Models.Events;
using CSharpSnookerCore.Models;
using System;

namespace CSharpSnooker.WinForms.Components
{
    class CollisionManager
    {
        public delegate void MyEventHandler(BallsCollisionEventArgs e);
        public event MyEventHandler OnBallsCollision;

        public void ResolveCollision(Ball ball1, Ball ball2)
        {
            // Get the mtd
            Vector2D delta = (ball1.Position.Subtract(ball2.Position));
            double d = delta.Lenght();

            // Minimum translation distance to push balls apart after intersecting
            Vector2D mtd = delta.Multiply((((Ball.Radius + 1.0 + Ball.Radius + 1.0) - d) / d));

            // resolve intersection --
            // inverse mass quantities
            double im1 = 1f;
            double im2 = 1f;

            // Push-pull them apart based off their mass
            ball1.Position = ball1.Position.Add((mtd.Multiply(im1 / (im1 + im2))));
            ball2.Position = ball2.Position.Subtract(mtd.Multiply(im2 / (im1 + im2)));

            // Impact speed
            Vector2D v = (ball1.Velocity.Subtract(ball2.Velocity));
            double vn = v.Dot(mtd.Normalize());

            // Sphere intersecting but moving away from each other already
            if (vn > 0.0d)
                return;

            // Collision impulse
            double i = Math.Abs(((-(1.0f + 0.1) * vn) / (im1 + im2)));
            Vector2D impulse = mtd.Multiply(1);

            OnBallsCollision?.Invoke(new BallsCollisionEventArgs(ball1, impulse));

            // change in momentum
            ball1.Velocity = ball1.Velocity.Add(impulse.Multiply(im1));
            ball2.Velocity = ball2.Velocity.Subtract(impulse.Multiply(im2));
        }

        public void ResolveCollision(Ball ball, DiagonalBorder diagonalBorder)
        {
            int y = 0;
            int x = 0;
            int deltaX = 0;

            Vector2D maxPoint = new Vector2D(0, 0);
            switch (diagonalBorder.Side)
            {
                case Side.Southeast:
                    y = (int)(-ball.X);
                    maxPoint = new Vector2D((ball.X + Ball.CosBall45) - 1, (ball.Y + Ball.CosBall45) - 1);
                    break;
                case Side.Northwest:
                    y = (int)(ball.X);
                    maxPoint = new Vector2D((ball.X - Ball.CosBall45) + 1, (ball.Y - Ball.CosBall45) + 1);
                    break;
                case Side.Northeast:
                    y = (int)(ball.X);
                    maxPoint = new Vector2D((ball.X + Ball.CosBall45) - 1, (ball.Y - Ball.CosBall45) + 1);
                    break;
                case Side.Southwest:
                    y = (int)(ball.X);
                    maxPoint = new Vector2D(ball.X - Ball.CosBall45 + 2, (ball.Y + Ball.CosBall45) - 2);
                    break;
            }

            x = Math.Abs((int)(diagonalBorder.Y1 - maxPoint.Y + diagonalBorder.X1));
            deltaX = Math.Abs((int)(maxPoint.X - x));

            int offSet = (int)(deltaX / (1 / Math.Cos(Math.PI / 4)));
            int offSetX = (int)(Math.Cos(Math.PI / 4) * offSet);

            Vector2D position = maxPoint;

            // get the mtd
            Vector2D delta = (position.Subtract(ball.Position));
            double d = delta.Lenght();

            // minimum translation distance to push balls apart after intersecting
            Vector2D mtd = delta.Multiply(((Ball.Radius) - d) / d);

            // resolve intersection --
            // inverse mass quantities
            double im1 = 0.5d;
            double im2 = 0.5d;

            // push-pull them apart based off their mass2
            ball.Position = ball.Position.Subtract(mtd.Multiply(im2 / (im1 + im2)));

            // impact speed
            Vector2D v = ball.Velocity.Multiply(-1);
            double vn = v.Dot(mtd.Normalize());

            // sphere intersecting but moving away from each other already
            if (vn > 0.0d)
                return;

            // collision impulse
            double i = Math.Abs((-(1.0f + 0.3) * vn) / (im1 + im2));
            Vector2D impulse = mtd.Multiply(i);

            // change in momentum
            ball.Velocity = ball.Velocity.Subtract(impulse.Multiply(im2));
        }

        public void ResolveCollision(Ball ball, TableBorder tableBorder, RectangleCollision collision)
        {
            double absorption = 0.9d;

            if (tableBorder.Direction == ForcedDirection.None)
            {
                switch (collision)
                {
                    case RectangleCollision.Right:
                    case RectangleCollision.Left:
                        if (Math.Sign(ball.Velocity.X) == Math.Sign(ball.VSpinVelocity.X) && ball.VSpinVelocity.X > 0.0)
                        {
                            ball.Velocity.X += ball.VSpinVelocity.X;
                            ball.VSpinVelocity.X = 0.0;
                        }
                        ball.Velocity.X *= -1.0d * absorption;
                        break;
                    case RectangleCollision.Bottom:
                    case RectangleCollision.Top:
                        if (Math.Sign(ball.Velocity.Y) == Math.Sign(ball.VSpinVelocity.Y) && ball.VSpinVelocity.Y > 0.0)
                        {
                            ball.Velocity.Y += ball.VSpinVelocity.Y;
                            ball.VSpinVelocity.Y = 0.0;
                        }
                        ball.Velocity.Y *= -1.0d * absorption;
                        break;
                }
            }
            else
            {
                Vector2D position = new Vector2D(tableBorder.X + tableBorder.Width / 2, tableBorder.Y + tableBorder.Height / 2);

                switch (tableBorder.Direction)
                {
                    case ForcedDirection.Up:
                        ball.Velocity.Y *= -0.5d;
                        ball.Velocity.X = ball.Velocity.Y * -0.5d;
                        break;
                    case ForcedDirection.Down:
                        ball.Velocity.Y *= -0.5d;
                        ball.Velocity.X = ball.Velocity.Y * -0.5d;
                        break;
                }
            }
        }
    }
}
