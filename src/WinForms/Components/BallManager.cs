using CSharpSnookerCore.Models;
using System;
using System.Collections.Generic;

namespace CSharpSnooker.WinForms.Components
{
    class BallManager
    {
        public List<Ball> Balls { get; private set; }
        public List<Ball> PottedBalls { get; private set; }
        public List<Ball> FallenBalls { get; private set; }
        public List<Ball> StrokenBalls { get; private set; }
        public Ball CueBall => Balls[0];



        public BallManager()
        {
            Load(new Ball[1]);
        }



        public void Load(Ball[] balls)
        {
            Balls = new List<Ball>(balls);

            StrokenBalls = new List<Ball>();
            FallenBalls = new List<Ball>();
            PottedBalls = new List<Ball>();
        }

        public bool AreColliding(Ball ball1, Ball ball2)
        {
            if (!ball1.IsInPocket && !ball2.IsInPocket)
            {
                float xd = (float)(ball2.Position.X - ball1.X);
                float yd = (float)(ball2.Position.Y - ball1.Y);

                float sumRadius = (float)((Ball.Radius + 1.0) * 2);
                float sqrRadius = sumRadius * sumRadius;

                float distSqr = (xd * xd) + (yd * yd);

                if (Math.Round(distSqr) < Math.Round(sqrRadius))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the ball in the specific coordinates if exists.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Null if no ball in the given position.</returns>
        public Ball GetBallOn(int x, int y)
        {
            Ball bOn = null;

            foreach (Ball ball in Balls)
            {
                if (!ball.IsInPocket && ball.Points > 1)
                {
                    float xd = (float)(x - ball.X);
                    float yd = (float)(y - ball.Y);

                    float sumRadius = (float)(Ball.Radius);
                    float sqrRadius = sumRadius * sumRadius;

                    float distSqr = (xd * xd) + (yd * yd);

                    if (Math.Round(distSqr) < Math.Round(sqrRadius))
                    {
                        bOn = ball;
                        break;
                    }
                }
            }

            return bOn;
        }
    }
}
