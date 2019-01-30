using System.Drawing;

namespace CSharpSnookerCore.Models
{
    public class BallPosition
    {
        public int SnapShot { get; }
        public int X { get; }
        public int Y { get; }
        public Image Image { get; }



        public BallPosition(int snapShot, Image image, int x, int y)
        {
            SnapShot = snapShot;
            Image = image;
            X = x;
            Y = y;
        }
    }
}
