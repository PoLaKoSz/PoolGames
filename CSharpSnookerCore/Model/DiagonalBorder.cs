using System;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using System.Text;

namespace CSharpSnookerCore.Model
{
    public class DiagonalBorder
    {
        #region attributes
        int x1 = 0;
        int y1 = 0;
        int x2 = 0;
        int y2 = 0;
        int width = 0;
        Side side = Side.Northeast;
        public static string message;
        #endregion attributes

        #region constructor
        public DiagonalBorder(int x1, int y1, int width, Side side)
        {
            this.x1 = x1;
            this.y1 = y1;

            switch (side)
            {
                case Side.Northeast:
                case Side.Southwest:
                    x2 = x1 + width;
                    y2 = y1 + width;
                    break;
                case Side.Northwest:
                case Side.Southeast:
                    x2 = x1 + width;
                    y2 = y1 - width;
                    break;
            }
            this.width = width;
            this.side = side;
        }
        #endregion constructor

        #region properties
        public int X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        
        public int Y1
        {
            get { return y1; }
            set { y1 = value; }
        }

        public int X2
        {
            get { return x2; }
        }

        public int Y2
        {
            get { return y2; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public Side Side
        {
            get { return side; }
            set { side = value; }
        }
        #endregion properties

        #region functions
        public bool Colliding(Ball ball)
        {
            int baseX = x1;
            int baseY = y1 - width;

            if (!ball.IsBallInPocket)
            {
                if (side == Side.Southeast)
                {
                    int x = (int)ball.X + (int)Ball.Radius;
                    int y = (int)ball.Y + (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X + Ball.CosBall45) - 1, (double)(ball.Y + Ball.CosBall45) - 1);

                    if ((x - baseX + y - baseY >= width) && (maxPoint.X >= x1 && maxPoint.X <= x2 && maxPoint.Y >= y2 && maxPoint.Y <= y1))
                    {
                        return true;
                    }
                }
                else if (side == Side.Northwest)
                {
                    int x = (int)ball.X - (int)Ball.Radius;
                    int y = (int)ball.Y - (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X - Ball.CosBall45) - 1, (double)(ball.Y - Ball.CosBall45) - 1);

                    if ((x - baseX + y - baseY <= width) && (maxPoint.X >= x1 && maxPoint.X <= x2 && maxPoint.Y >= y2 && maxPoint.Y <= y1))
                    {
                        return true;
                    }
                }
                else if (side == Side.Northeast)
                {
                    int x = (int)ball.X + (int)Ball.Radius;
                    int y = (int)ball.Y - (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X + Ball.CosBall45) - 1, (double)(ball.Y - Ball.CosBall45) - 1);

                    if ((x - baseX + y2 - y >= width) && (maxPoint.X >= x1 && maxPoint.X <= x2 && maxPoint.Y >= y1 && maxPoint.Y <= y2))
                    {
                        return true;
                    }
                }
                else if (side == Side.Southwest)
                {
                    int x = (int)ball.X - (int)Ball.Radius;
                    int y = (int)ball.Y + (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X - Ball.CosBall45) - 1, (double)(ball.Y + Ball.CosBall45) - 1);

                    if ((x - baseX + y2 - y <= width) && (maxPoint.X >= x1 && maxPoint.X <= x2 && maxPoint.Y >= y1 && maxPoint.Y <= y2))
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
            switch (this.side)
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

            x = Math.Abs((int)(y1 - maxPoint.Y + x1));
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
            return string.Format("DiagonalBorder({0}, {1}, {2}, {3})", x1, y1, x2, y2);
        }
        #endregion functions
    }

    public enum Side
    {
        Northeast,
        Southeast,
        Southwest,
        Northwest
    }
}
