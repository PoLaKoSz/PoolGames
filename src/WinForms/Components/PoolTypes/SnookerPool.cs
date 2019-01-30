using System;
using System.Collections.Generic;
using System.Drawing;
using CSharpSnookerCore.Models;

namespace CSharpSnooker.WinForms.Components.PoolTypes
{
    class SnookerPool : IPoolType
    {
        public Ball[] Balls { get; }

        /// <summary>
        /// If it's true, than somebody won the game.
        /// </summary>
        public bool HasWinner { get; private set; }

        private PlayerManager _playerManager;



        public SnookerPool()
        {
            Image imgRedBall    = Image.FromFile(@"Images\RedBall.PNG");
            Image imgWhiteBall  = Image.FromFile(@"Images\whiteball.PNG");
            Image imgYellowBall = Image.FromFile(@"Images\YellowBall.PNG");
            Image imgGreenBall  = Image.FromFile(@"Images\GreenBall.PNG");
            Image imgBrownBall  = Image.FromFile(@"Images\BrownBall.PNG");
            Image imgBlackBall  = Image.FromFile(@"Images\BlackBall.PNG");
            Image imgPinkBall   = Image.FromFile(@"Images\PinkBall.PNG");
            Image imgBlueBall   = Image.FromFile(@"Images\BlueBall.PNG");

            Balls = new Ball[]
            {
                new Ball("white", 497, 140, imgWhiteBall, points: 0),

                new Ball("red01", 121, 152, imgRedBall, points: 1),
                new Ball("red02", 121, 171, imgRedBall, 1),
                new Ball("red03", 121, 190, imgRedBall, 1),
                new Ball("red04", 140, 162, imgRedBall, 1),
                new Ball("red05", 140, 180, imgRedBall, 1),
                new Ball("red06", 159, 171, imgRedBall, 1),

                new Ball("yellow", 469, 115, imgYellowBall, points: 2),
                new Ball("green",  469, 228, imgGreenBall,  3),
                new Ball("brown",  469, 171, imgBrownBall,  4),
                new Ball("blue",   298, 171, imgBlueBall,   5),
                new Ball("pink",   178, 171, imgPinkBall,   6),
                new Ball("black",   50, 171, imgBlackBall,  7),
            };
        }



        public void InitBallOn(PlayerManager playerManager)
        {
            _playerManager = playerManager;

            _playerManager.CurrentPlayer.BallOn = GetRandomRedBall();
            _playerManager.OtherPlayer.BallOn = GetRandomRedBall();
        }

        public void ProcessFallenBalls(BallManager ballManager, PlayerManager playerManager)
        {
            playerManager.CurrentPlayer.FoulList.Clear();

            int redCount = 0;
            int fallenRedCount = 0;
            int availableRedCount = 0;
            int wonPoints = 0;
            int lostPoints = 0;
            bool someInTable = false;

            foreach (Ball ball in ballManager.Balls)
            {
                if (!ball.IsInPocket)
                {
                    if (ball.Points > 0)
                    {
                        someInTable = true;
                    }
                }

                if (ball.Points == 1)
                {
                    redCount++;
                }

                if (ball.Points == 1 && ball.IsInPocket)
                {
                    fallenRedCount++;
                }
            }

            availableRedCount = redCount - fallenRedCount;

            foreach (Ball ball in ballManager.PottedBalls)
            {
                if (ball.Points == 0)
                {
                    ball.ResetPositionAt(ball.InitPosition.X, ball.InitPosition.Y);
                    ball.IsInPocket = false;
                }
                else if (ball.Points > 1)
                {
                    int ballOnPoints = 1;
                    if (playerManager.CurrentPlayer.BallOn != null)
                    {
                        ballOnPoints = playerManager.CurrentPlayer.BallOn.Points;
                    }

                    if (fallenRedCount < redCount || ballOnPoints != ball.Points)
                    {
                        for (int points = ball.Points; points > 1; points--)
                        {
                            Ball candidateBall = GetCandidateBall(ball, points);
                            if (candidateBall != null)
                            {
                                ball.ResetPositionAt(candidateBall.InitPosition.X, candidateBall.InitPosition.Y);
                                ball.IsInPocket = false;
                                break;
                            }
                        }
                    }
                }
            }

            int strokenBallsCount = 0;

            foreach (Ball ball in ballManager.StrokenBalls)
            {
                // Causing the cue ball to first hit a ball other than the ball on
                if (strokenBallsCount == 0 && ball.Points != playerManager.CurrentPlayer.BallOn.Points)
                {
                    playerManager.CurrentPlayer.FoulList.Add((playerManager.CurrentPlayer.BallOn.Points < 4 ? 4 : playerManager.CurrentPlayer.BallOn.Points));
                }

                strokenBallsCount++;
            }

            // Foul: causing the cue ball to miss all object balls
            if (strokenBallsCount == 0)
            {
                playerManager.CurrentPlayer.FoulList.Add(4);
            }

            foreach (Ball ball in ballManager.PottedBalls)
            {
                // Causing the cue ball to enter a pocket
                if (ball.Points == 0)
                {
                    playerManager.CurrentPlayer.FoulList.Add(4);
                }

                // Causing: this is not the BallOn in the Pocket
                if (ball.Points != playerManager.CurrentPlayer.BallOn.Points)
                {
                    playerManager.CurrentPlayer.FoulList.Add(playerManager.CurrentPlayer.BallOn.Points < 4 ? 4 : playerManager.CurrentPlayer.BallOn.Points);
                }
            }

            if (playerManager.CurrentPlayer.FoulList.Count == 0)
            {
                foreach (Ball ball in ballManager.PottedBalls)
                {
                    // Legally potting reds or colors
                    wonPoints += ball.Points;
                }
            }
            else
            {
                playerManager.CurrentPlayer.FoulList.Sort();
                lostPoints = playerManager.CurrentPlayer.FoulList[playerManager.CurrentPlayer.FoulList.Count - 1];
            }

            if (wonPoints == 0 || lostPoints > 0)
            {
                ChooseNextBallOn(ballManager, playerManager, availableRedCount, isLostBreak: true);

                playerManager.CurrentPlayer.Points -= lostPoints;
                playerManager.OtherPlayer.Points += lostPoints;

                playerManager.Switch();
            }
            else
            {
                ChooseNextBallOn(ballManager, playerManager, availableRedCount, isLostBreak: false);

                playerManager.CurrentPlayer.Points += wonPoints;
            }

            if (!someInTable)
            {
                HasWinner = true;
                return;
            }

            int fallenBallsCount = ballManager.FallenBalls.Count;
            for (int i = fallenBallsCount - 1; i >= 0; i--)
            {
                if (!ballManager.FallenBalls[i].IsInPocket)
                {
                    ballManager.FallenBalls.RemoveAt(i);
                }
            }

            ballManager.StrokenBalls.Clear();
            ballManager.PottedBalls.Clear();
        }


        private void ChooseNextBallOn(BallManager ballManager, PlayerManager playerManager, int availableRedCount, bool isLostBreak)
        {
            // Ha van még piros labra
            //      - és az előző BallOn nem piros volt
            //      - és az előző BallOn piros volt
            // Ha nincs piros labda, akkor a legkisebb színes

            if (isLostBreak && 0 < availableRedCount)
            {
                playerManager.CurrentPlayer.BallOn = GetRandomRedBall();
            }
            else if (isLostBreak && 0 == availableRedCount)
            {
                playerManager.CurrentPlayer.BallOn = GetMinColouredball();
            }
            else if (0 < availableRedCount && playerManager.CurrentPlayer.BallOn.Points != 1)
            {
                playerManager.CurrentPlayer.BallOn = GetRandomRedBall();
            }
            else if (0 < availableRedCount && playerManager.CurrentPlayer.BallOn.Points == 1)
            {
                if (!playerManager.CurrentPlayer.IsComputer)
                    playerManager.CurrentPlayer.BallOn = null;
                else
                    playerManager.CurrentPlayer.BallOn = GetMinColouredball();
            }
            else if (0 == availableRedCount)
            {
                playerManager.CurrentPlayer.BallOn = GetMinColouredball();
            }
        }

        /// <summary>
        /// Gets the first not potted coloured ball.
        /// </summary>
        /// <returns>Non null object.</returns>
        private Ball GetMinColouredball()
        {
            Ball minColouredball = null;

            int minPoints = int.MaxValue;

            foreach (Ball ball in Balls)
            {
                if (1 < ball.Points && ball.Points < minPoints && !ball.IsInPocket)
                {
                    minColouredball = ball;
                    minPoints = minColouredball.Points;
                }
            }

            return minColouredball;
        }

        private List<Ball> GetValidRedBalls()
        {
            List<Ball> validRedBalls = new List<Ball>();

            foreach (Ball ball in Balls)
            {
                if (ball.Points == 1 && !ball.IsInPocket)
                {
                    validRedBalls.Add(ball);
                }
            }

            return validRedBalls;
        }

        private Ball GetCandidateBall(Ball ball, int points)
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
                        if (!collisionBall.IsInPocket)
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

        private Ball GetRandomRedBall()
        {
            Ball redBallOn = null;

            List<int> validRedBalls = new List<int>();
            int i = 0;
            foreach (Ball ball in Balls)
            {
                if (ball.Points == 1 && !ball.IsInPocket)
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

        private Ball GetNextBallOn(Player player)
        {
            Ball nextBallOn = null;

            if (player.BallOn == null || player.BallOn.Points == 1)
            {
                nextBallOn = GetRandomRedBall();
            }
            else
            {
                nextBallOn = GetMinColouredball();
            }

            return nextBallOn;
        }
    }
}
