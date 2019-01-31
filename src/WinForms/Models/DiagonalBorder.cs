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
