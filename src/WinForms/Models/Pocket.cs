using System;

namespace CSharpSnookerCore.Models
{
    public class Pocket
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int HotSpotX { get; set; }
        public int HotSpotY { get; set; }


        private readonly IPocketObserver subscriber;



        public Pocket(IPocketObserver subscriber, int id, int x, int y, int hotSpotX, int hotSpotY)
        {
            Id = id;
            X = (int)(x + Ball.Radius + 4);
            Y = (int)(y + Ball.Radius + 4);
            HotSpotX = hotSpotX;
            HotSpotY = hotSpotY;
            this.subscriber = subscriber;
        }



        public bool IsBallInPocket(Ball ball)
        {
            float xd = (float)(X - ball.X);
            float yd = (float)(Y - ball.Y);
            
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
                    ball.Position.X = X;
                    ball.Position.Y = Y;
                }

                ball.TranslateVelocity.X = 0;
                ball.TranslateVelocity.Y = 0;
                return true;
            }

            return false;
        }
    }
}
