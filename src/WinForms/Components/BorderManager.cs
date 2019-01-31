using CSharpSnooker.WinForms.Models.Events;
using CSharpSnookerCore.Models;
using System.Collections.Generic;

namespace CSharpSnooker.WinForms.Components
{
    /// <summary>
    /// Class to hold borders and detect if a <see cref="Ball"/> is colliding with them.
    /// </summary>
    class BorderManager
    {
        public delegate void MyEventHandler(BorderCollisionEventArgs e);
        
        /// <summary>
        /// Fires an event when a <see cref="Ball"/> is colliding with a border.
        /// </summary>
        public event MyEventHandler OnCollision;

        public List<TableBorder> TableBorders { get; }
        public List<DiagonalBorder> DiagonalBorders { get; }



        public BorderManager()
        {
            DiagonalBorders = new List<DiagonalBorder>()
            {
                new DiagonalBorder(547, 309, 35, Side.Southwest),
                new DiagonalBorder(573, 286, 35, Side.Northeast),
                new DiagonalBorder(  1,  27, 35, Side.Southwest),
                new DiagonalBorder( 24,   1, 35, Side.Northeast),
                new DiagonalBorder(546,  33, 35, Side.Northwest),
                new DiagonalBorder(567,  59, 35, Side.Southeast),
                new DiagonalBorder(  1, 319, 35, Side.Northwest),
                new DiagonalBorder( 18, 344, 35, Side.Southeast),
            };

            TableBorders = new List<TableBorder>()
            {
                new TableBorder(  0,  55,  27, 235, ForcedDirection.None),
                new TableBorder(577,  55,  27, 235, ForcedDirection.None),
                new TableBorder( 51,   0, 230,  27, ForcedDirection.None),
                new TableBorder( 51, 316, 230,  27, ForcedDirection.None),
                new TableBorder(319,   0, 235,  27, ForcedDirection.None),
                new TableBorder(319, 316, 235,  27, ForcedDirection.None),

                new TableBorder(-20,  55,  20, 289, ForcedDirection.None),
                new TableBorder(606,  55,  20, 289, ForcedDirection.None),
                new TableBorder(  0, -20, 606,  20, ForcedDirection.None),
                new TableBorder(  0, 344, 606,  20, ForcedDirection.None),
            };
        }



        public RectangleCollision CheckCollision(Ball ball, TableBorder tableBorder)
        {
            RectangleCollision collision = RectangleCollision.None;

            if (!ball.IsInPocket)
            {
                if (tableBorder.X < 288 && (ball.X - Ball.Radius < tableBorder.X + tableBorder.Width) && (ball.Y >= tableBorder.Y && ball.Y <= tableBorder.Y + tableBorder.Height) && (ball.Velocity.X + ball.VSpinVelocity.X < 0.0d) && (ball.LastX > tableBorder.X + tableBorder.Width))
                {
                    collision = RectangleCollision.Right;
                }
                else if (tableBorder.X > 288 && (ball.X + Ball.Radius > tableBorder.X) && (ball.Y >= tableBorder.Y && ball.Y <= tableBorder.Y + tableBorder.Height) && (ball.Velocity.X + ball.VSpinVelocity.X > 0.0d) && (ball.LastX < tableBorder.X))
                {
                    collision = RectangleCollision.Left;
                }

                if (tableBorder.Y < 161 && (ball.Y - Ball.Radius < tableBorder.Y + tableBorder.Height) && (ball.X >= tableBorder.X && ball.X - Ball.Radius <= tableBorder.X + tableBorder.Width) && (ball.Velocity.Y + ball.VSpinVelocity.Y < 0.0d) && (ball.LastY > tableBorder.Y) && (ball.LastX < tableBorder.X + tableBorder.Width))
                {
                    collision = RectangleCollision.Bottom;
                }
                else if (tableBorder.Y > 161 && (ball.Y + Ball.Radius > tableBorder.Y) && (ball.X >= tableBorder.X && ball.X <= tableBorder.X + tableBorder.Width) && (ball.Velocity.Y + ball.VSpinVelocity.Y > 0.0d) && (ball.LastY < tableBorder.Y) && (ball.LastY < tableBorder.Y) && (ball.LastX < tableBorder.X + tableBorder.Width))
                {
                    collision = RectangleCollision.Top;
                }
            }

            if (collision != RectangleCollision.None)
            {
                OnCollision?.Invoke(new BorderCollisionEventArgs(ball));
            }

            return collision;
        }
        
        public bool CheckCollision(Ball ball, DiagonalBorder diagonalBorder)
        {
            int baseX = diagonalBorder.X1;
            int baseY = diagonalBorder.Y1 - diagonalBorder.Width;

            if (!ball.IsInPocket)
            {
                if (diagonalBorder.Side == Side.Southeast)
                {
                    int x = (int)ball.X + (int)Ball.Radius;
                    int y = (int)ball.Y + (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X + Ball.CosBall45) - 1, (double)(ball.Y + Ball.CosBall45) - 1);

                    if ((x - baseX + y - baseY >= diagonalBorder.Width) && (maxPoint.X >= diagonalBorder.X1 && maxPoint.X <= diagonalBorder.X2 && maxPoint.Y >= diagonalBorder.Y2 && maxPoint.Y <= diagonalBorder.Y1))
                    {
                        return true;
                    }
                }
                else if (diagonalBorder.Side == Side.Northwest)
                {
                    int x = (int)ball.X - (int)Ball.Radius;
                    int y = (int)ball.Y - (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X - Ball.CosBall45) - 1, (double)(ball.Y - Ball.CosBall45) - 1);

                    if ((x - baseX + y - baseY <= diagonalBorder.Width) && (maxPoint.X >= diagonalBorder.X1 && maxPoint.X <= diagonalBorder.X2 && maxPoint.Y >= diagonalBorder.Y2 && maxPoint.Y <= diagonalBorder.Y1))
                    {
                        return true;
                    }
                }
                else if (diagonalBorder.Side == Side.Northeast)
                {
                    int x = (int)ball.X + (int)Ball.Radius;
                    int y = (int)ball.Y - (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X + Ball.CosBall45) - 1, (double)(ball.Y - Ball.CosBall45) - 1);

                    if ((x - baseX + diagonalBorder.Y2 - y >= diagonalBorder.Width) && (maxPoint.X >= diagonalBorder.X1 && maxPoint.X <= diagonalBorder.X2 && maxPoint.Y >= diagonalBorder.Y1 && maxPoint.Y <= diagonalBorder.Y2))
                    {
                        return true;
                    }
                }
                else if (diagonalBorder.Side == Side.Southwest)
                {
                    int x = (int)ball.X - (int)Ball.Radius;
                    int y = (int)ball.Y + (int)Ball.Radius;

                    Vector2D maxPoint = new Vector2D((double)(ball.X - Ball.CosBall45) - 1, (double)(ball.Y + Ball.CosBall45) - 1);

                    if ((x - baseX + diagonalBorder.Y2 - y <= diagonalBorder.Width) && (maxPoint.X >= diagonalBorder.X1 && maxPoint.X <= diagonalBorder.X2 && maxPoint.Y >= diagonalBorder.Y1 && maxPoint.Y <= diagonalBorder.Y2))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
