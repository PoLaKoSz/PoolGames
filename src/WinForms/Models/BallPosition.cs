using System;

namespace CSharpSnookerCore.Models
{
    public class BallPosition
    {
        public int SnapShot { get; set; }
        public int BallIndex { get; set; }
        public int X { get; set; }
        public int Y { get; set; }



        public BallPosition(int snapShot, int ballIndex, int x, int y)
        {
            SnapShot = snapShot;
            BallIndex = ballIndex;
            X = x;
            Y = y;
        }
    }
}
