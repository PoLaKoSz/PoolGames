using CSharpSnookerCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CSharpSnooker.WinForms.Components
{
    class BallManager
    {
        public List<Ball> Balls { get; private set; }
        public List<Ball> PottedBalls { get; private set; }
        public List<Ball> FallenBalls { get; private set; }
        public List<Ball> StrokenBalls { get; private set; }



        public BallManager(MainForm mainForm)
        {
            Load(mainForm);
            StrokenBalls = new List<Ball>();
            FallenBalls = new List<Ball>();
            PottedBalls = new List<Ball>();
        }



        public void Load(MainForm mainForm)
        {
            Image imgRedBall    = Image.FromFile(@"Images\RedBall.PNG");
            Image imgWhiteBall  = Image.FromFile(@"Images\whiteball.PNG");
            Image imgYellowBall = Image.FromFile(@"Images\YellowBall.PNG");
            Image imgGreenBall  = Image.FromFile(@"Images\GreenBall.PNG");
            Image imgBrownBall  = Image.FromFile(@"Images\BrownBall.PNG");
            Image imgBlackBall  = Image.FromFile(@"Images\BlackBall.PNG");
            Image imgPinkBall   = Image.FromFile(@"Images\PinkBall.PNG");
            Image imgBlueBall   = Image.FromFile(@"Images\BlueBall.PNG");

            Balls = new List<Ball>()
            {
                new Ball("white", mainForm, 497, 140, imgWhiteBall, points: 0),

                new Ball("red01", mainForm, 121, 152, imgRedBall, points: 1),
                new Ball("red02", mainForm, 121, 171, imgRedBall, 1),
                new Ball("red03", mainForm, 121, 190, imgRedBall, 1),
                new Ball("red04", mainForm, 140, 162, imgRedBall, 1),
                new Ball("red05", mainForm, 140, 180, imgRedBall, 1),
                new Ball("red06", mainForm, 159, 171, imgRedBall, 1),

                new Ball("yellow", mainForm, 469, 115, imgYellowBall, points: 2),
                new Ball("green",  mainForm, 469, 228, imgGreenBall,  3),
                new Ball("brown",  mainForm, 469, 171, imgBrownBall,  4),
                new Ball("blue",   mainForm, 298, 171, imgBlueBall,   5),
                new Ball("pink",   mainForm, 178, 171, imgPinkBall,   6),
                new Ball("black",  mainForm,  50, 171, imgBlackBall,  7),
            };
        }

        /// <summary>
        /// Gets the first not potted coloured ball.
        /// </summary>
        /// <returns>Non null object.</returns>
        public Ball GetMinColouredball()
        {
            Ball minColouredball = null;

            int minPoints = int.MaxValue;

            foreach (Ball ball in Balls)
            {
                if (1 < ball.Points && ball.Points < minPoints && !ball.IsBallInPocket)
                {
                    minColouredball = ball;
                    minPoints = minColouredball.Points;
                }
            }

            return minColouredball;
        }

        public List<Ball> GetValidRedBalls()
        {
            List<Ball> validRedBalls = new List<Ball>();

            foreach (Ball ball in Balls)
            {
                if (ball.Points == 1 && !ball.IsBallInPocket)
                {
                    validRedBalls.Add(ball);
                }
            }

            return validRedBalls;
        }

        /// <summary>
        /// Gets the ball in the specific coordinates if exists.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Could be null.</returns>
        public Ball GetBallOn(int x, int y)
        {
            Ball bOn = null;

            foreach (Ball ball in Balls)
            {
                if (!ball.IsBallInPocket && ball.Points > 1)
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
        
        public Ball GetCandidateBall(Ball ball, int points)
        {
            Ball candidateBall = null;
            Ball fallenBall = ball;
            while (candidateBall == null)
            {
                foreach (Ball b in Balls)
                {
                    if (b.Points == points)
                    {
                        candidateBall = b;
                    }
                }
                if (candidateBall != null)
                {
                    foreach (Ball collisionBall in Balls)
                    {
                        if (!collisionBall.IsBallInPocket)
                        {
                            if (collisionBall.Id != candidateBall.Id)
                            {
                                float xd = (float)(candidateBall.InitPosition.X - collisionBall.X);
                                float yd = (float)(candidateBall.InitPosition.Y - collisionBall.Y);

                                float sumRadius = (float)(Ball.Radius * 0.5);
                                float sqrRadius = sumRadius * sumRadius;

                                float distSqr = (xd * xd) + (yd * yd);

                                if (Math.Round(distSqr) < Math.Round(sqrRadius))
                                {
                                    candidateBall = null;
                                    points--;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return candidateBall;
        }

        public Ball GetRandomRedBall()
        {
            Ball redBallOn = null;

            List<int> validRedBalls = new List<int>();
            int i = 0;
            foreach (Ball ball in Balls)
            {
                if (ball.Points == 1 && !ball.IsBallInPocket)
                {
                    validRedBalls.Add(i);
                }
                i++;
            }

            int redCount = validRedBalls.Count;

            if (redCount > 0)
            {
                Random rnd = new Random(DateTime.Now.Second);
                int index = rnd.Next(redCount);

                redBallOn = Balls[validRedBalls[index]];
            }
            return redBallOn;
        }
    }
}
