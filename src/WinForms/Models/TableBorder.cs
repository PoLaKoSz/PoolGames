using System;

namespace CSharpSnookerCore.Models
{
    public class TableBorder
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ForcedDirection Direction { get; }



        public TableBorder(int x, int y, int width, int height, ForcedDirection direction)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Direction = direction;
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
