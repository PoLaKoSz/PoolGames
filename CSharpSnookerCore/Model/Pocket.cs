using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnookerCore.Model
{
    public class Pocket
    {
        #region attributes
        int id = 0;
        int x = 0;
        int y = 0;
        int hotSpotX = 0;
        int hotSpotY = 0;
        IPocketObserver subscriber;
        #endregion attributes

        #region constructor
        public Pocket(IPocketObserver subscriber, int id, int x, int y, int hotSpotX, int hotSpotY)
        {
            this.id = id;
            this.x = (int)(x + Ball.Radius + 4);
            this.y = (int)(y + Ball.Radius + 4);
            this.hotSpotX = hotSpotX;
            this.hotSpotY = hotSpotY;
            this.subscriber = subscriber;
        }
        #endregion constructor
        
        #region properties
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

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

        public int HotSpotX
        {
            get { return hotSpotX; }
            set { hotSpotX = value; }
        }

        public int HotSpotY
        {
            get { return hotSpotY; }
            set { hotSpotY = value; }
        }
        #endregion properties

        #region functions
        public bool IsBallInPocket(Ball ball)
        {
            float xd = (float)(x - ball.X);
            float yd = (float)(y - ball.Y);
            
            float sumRadius = (float)(Ball.Radius * 1.5);
            float sqrRadius = sumRadius * sumRadius;

            float distSqr = (xd * xd) + (yd * yd);

            if (Math.Round(distSqr) < Math.Round(sqrRadius))
            {
                if (!ball.IsBallInPocket)
                {
                    subscriber.BallDropped(ball);
                }

                if (ball.Position.X != ball.LastX || ball.Position.Y != ball.LastY)
                    ball.IsBallInPocket = true;

                if (ball.Id != "01")
                {
                    ball.Position.X = x;
                    ball.Position.Y = y;
                }

                ball.TranslateVelocity.X = 0;
                ball.TranslateVelocity.Y = 0;
                return true;
            }

            return false;
        }
        #endregion functions
    }
}
