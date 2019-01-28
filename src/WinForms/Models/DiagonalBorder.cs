using System;

namespace CSharpSnookerCore.Models
{
    public class DiagonalBorder
    {
        public int X1 { get; }
        public int Y1 { get; }
        public int X2 { get; }
        public int Y2 { get; }
        public int Width { get; }
        public Side Side { get; }


        public static string message;



        public DiagonalBorder(int x1, int y1, int width, Side side)
        {
            X1 = x1;
            Y1 = y1;

            switch (side)
            {
                case Side.Northeast:
                case Side.Southwest:
                    X2 = x1 + width;
                    Y2 = y1 + width;
                    break;
                case Side.Northwest:
                case Side.Southeast:
                    X2 = x1 + width;
                    Y2 = y1 - width;
                    break;
            }
            Width = width;
            Side = side;
        }



        public bool Colliding(Ball ball)
        {
            int baseX = X1;
            int baseY = Y1 - Width;

            if (!ball.IsBallInPocket)
            {
                if (Side == Side.Southeast)
                {
                    int x = (int)ball.X + (int)Ball.Radius;
                    int y = (int)ball.Y + (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X + Ball.CosBall45) - 1, (double)(ball.Y + Ball.CosBall45) - 1);

                    if ((x - baseX + y - baseY >= Width) && (maxPoint.X >= X1 && maxPoint.X <= X2 && maxPoint.Y >= Y2 && maxPoint.Y <= Y1))
                    {
                        return true;
                    }
                }
                else if (Side == Side.Northwest)
                {
                    int x = (int)ball.X - (int)Ball.Radius;
                    int y = (int)ball.Y - (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X - Ball.CosBall45) - 1, (double)(ball.Y - Ball.CosBall45) - 1);

                    if ((x - baseX + y - baseY <= Width) && (maxPoint.X >= X1 && maxPoint.X <= X2 && maxPoint.Y >= Y2 && maxPoint.Y <= Y1))
                    {
                        return true;
                    }
                }
                else if (Side == Side.Northeast)
                {
                    int x = (int)ball.X + (int)Ball.Radius;
                    int y = (int)ball.Y - (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X + Ball.CosBall45) - 1, (double)(ball.Y - Ball.CosBall45) - 1);

                    if ((x - baseX + Y2 - y >= Width) && (maxPoint.X >= X1 && maxPoint.X <= X2 && maxPoint.Y >= Y1 && maxPoint.Y <= Y2))
                    {
                        return true;
                    }
                }
                else if (Side == Side.Southwest)
                {
                    int x = (int)ball.X - (int)Ball.Radius;
                    int y = (int)ball.Y + (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X - Ball.CosBall45) - 1, (double)(ball.Y + Ball.CosBall45) - 1);

                    if ((x - baseX + Y2 - y <= Width) && (maxPoint.X >= X1 && maxPoint.X <= X2 && maxPoint.Y >= Y1 && maxPoint.Y <= Y2))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void ResolveCollision(Ball ball)
        {
            int y = 0;
            int x = 0;
            int deltaX = 0;

            Vector2D maxPoint = new Vector2D(0, 0);
            switch (this.Side)
            {
                case Side.Southeast:
                    y = (int)(-ball.X);
                    maxPoint = new Vector2D((double)(ball.X + Ball.CosBall45) - 1, (double)(ball.Y + Ball.CosBall45) - 1);
                    break;
                case Side.Northwest:
                    y = (int)(ball.X);
                    maxPoint = new Vector2D((double)(ball.X - Ball.CosBall45) + 1, (double)(ball.Y - Ball.CosBall45) + 1);
                    break;
                case Side.Northeast:
                    y = (int)(ball.X);
                    maxPoint = new Vector2D((double)(ball.X + Ball.CosBall45) - 1, (double)(ball.Y - Ball.CosBall45) + 1);
                    break;
                case Side.Southwest:
                    y = (int)(ball.X);
                    maxPoint = new Vector2D((double)(ball.X - Ball.CosBall45) + 2, (double)(ball.Y + Ball.CosBall45) - 2);
                    break;
            }

            x = Math.Abs((int)(Y1 - maxPoint.Y + X1));
            deltaX = Math.Abs((int)(maxPoint.X - x));

            int offSet = (int)(deltaX / (1 / Math.Cos(Math.PI / 4)));
            int offSetX = (int)(Math.Cos(Math.PI / 4) * offSet);

            Vector2D position = maxPoint;

            // get the mtd
            Vector2D delta = (position.Subtract(ball.Position));
            float d = delta.Lenght();
            // minimum translation distance to push balls apart after intersecting
            Vector2D mtd = delta.Multiply((float)(((Ball.Radius) - d) / d));

            // resolve intersection --
            // inverse mass quantities
            float im1 = 0.5f;
            float im2 = 0.5f;

            // push-pull them apart based off their mass2
            ball.Position = ball.Position.Subtract(mtd.Multiply(im2 / (im1 + im2)));

            // impact speed
            Vector2D v = ball.TranslateVelocity.Multiply(-1);
            float vn = v.Dot(mtd.Normalize());

            // sphere intersecting but moving away from each other already
            if (vn > 0.0f)
                return;

            // collision impulse
            float i = Math.Abs((float)((-(1.0f + 0.3) * vn) / (im1 + im2)));
            Vector2D impulse = mtd.Multiply(i);

            int hitSoundIntensity = (int)((Math.Abs(impulse.X) + Math.Abs(impulse.Y))/3);

            if (hitSoundIntensity > 2)
                hitSoundIntensity = 2;

            if (hitSoundIntensity < 1)
                hitSoundIntensity = 1;

            // change in momentum
            ball.TranslateVelocity = ball.TranslateVelocity.Subtract(impulse.Multiply(im2));
        }


        public override string ToString()
        {
            return string.Format("DiagonalBorder({0}, {1}, {2}, {3})", X1, Y1, X2, Y2);
        }
    }

    public enum Side
    {
        Northeast,
        Southeast,
        Southwest,
        Northwest
    }
}
