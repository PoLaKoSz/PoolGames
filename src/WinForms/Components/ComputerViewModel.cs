using CSharpSnookerCore.Models;
using System;
using System.Collections.Generic;

namespace CSharpSnooker.WinForms.Components
{
    class ComputerViewModel : PlayerViewModel
    {
        private readonly BallManager _ballManager;
        private readonly PocketManager _pocketManager;

        private static readonly int _maxBreakAttempts;
        private static readonly Random _random;



        static ComputerViewModel()
        {
            _maxBreakAttempts = 2;
            _random = new Random();
        }

        public ComputerViewModel(Player player, BallManager ballManager, PocketManager pocketManager)
            : base(player)
        {
            Model.Strength = GetRandomStrenght();
            _ballManager = ballManager;
            _pocketManager = pocketManager;
        }



        public override void GiveControl()
        {
            HitBall(
                new Vector2D((_random.Next(0) + 30), (_random.Next(0) + 30)),
                _ballManager.CueBall,
                new Vector2D(0, 0));

            //GenerateComputerShot();
        }

        public override Vector2D Hitting()
        {
            return new Vector2D(_random.Next(0, 600), _random.Next(1, 340));
        }


        private void GenerateComputerShot()
        {
            List<Ball> auxBalls = new List<Ball>();

            foreach (Ball b in _ballManager.Balls)
            {
                Ball auxBall = new Ball(b.Id, (int)b.Position.X, (int)b.Position.Y, b.Image, b.Points)
                {
                    IsInPocket = b.IsInPocket
                };
                auxBalls.Add(auxBall);
            }

            int lastPlayerScore = Model.Points;

            int newPlayerScore = -1;

            int attemptsToWin = 0;
            int attemptsNotToLose = 0;
            int attemptsOfDespair = 0;

            while (true)
            {
                if (attemptsToWin < _maxBreakAttempts)
                {
                    attemptsToWin++;
                }
                else if (attemptsNotToLose < _maxBreakAttempts)
                {
                    attemptsNotToLose++;
                }
                else
                {
                    attemptsOfDespair++;
                }

                Model.Points = lastPlayerScore;

                bool despair = (attemptsOfDespair >= _maxBreakAttempts);
                GenerateRandomTestComputerShot(despair);

                newPlayerScore = Model.Points;

                int i = 0;
                foreach (Ball b in _ballManager.Balls)
                {
                    Ball auxB = auxBalls[i];
                    b.Position.X = auxB.Position.X;
                    b.Position.Y = auxB.Position.Y;
                    b.IsInPocket = auxB.IsInPocket;
                    i++;
                }

                if (newPlayerScore > lastPlayerScore ||
                    attemptsToWin >= _maxBreakAttempts ||
                    attemptsOfDespair > 5)
                {
                    Model.Points = lastPlayerScore;

                    GenerateLastGoodComputerShot();
                    break;
                }
            }
        }

        private void GenerateRandomTestComputerShot(bool despair)
        {
            Model.BallOn = _ballManager.GetNextBallOn(Model);
            Ball ghostBall = null;

            List<Ball> ballOnList = new List<Ball>();

            if (Model.BallOn == null || Model.BallOn.Points == 1)
            {
                ballOnList = _ballManager.GetValidRedBalls();
            }
            else
            {
                Ball redBall = _ballManager.GetRandomRedBall();

                if (redBall != null)
                {
                    foreach (Ball b in _ballManager.Balls)
                    {
                        if (b.Points > 1 && !b.IsInPocket)
                            ballOnList.Add(b);
                    }
                }
                else
                {
                    ballOnList.Add(Model.BallOn);
                }
            }

            ghostBall = GetRandomGhostBall(ballOnList, despair);

            if (ghostBall == null)
                ghostBall = _ballManager.GetNextBallOn(Model);

            int strength = _random.Next(15);

            Model.Strength = 45 + strength;

            if (ghostBall != null)
            {
                Model.TestPosition = new Vector2D(ghostBall.X, ghostBall.Y);
                Model.TestStrength = Model.Strength;

                HitBall(Model.TestPosition, _ballManager.CueBall, new Vector2D(0, 0));
            }
        }

        private void GenerateLastGoodComputerShot()
        {
            Model.Strength = Model.TestStrength;

            HitBall(Model.TestPosition, _ballManager.CueBall, new Vector2D(0, 0));
        }

        private Ball GetRandomGhostBall(List<Ball> ballOnList, bool despair)
        {
            Ball randomGhostBall = null;

            List<Ball> ghostBalls = new List<Ball>();

            foreach (Ball ballOn in ballOnList)
            {
                List<Ball> tempGhostBalls = GetGhostBalls(ballOn, false);
                if (!despair)
                {
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                }
                else
                {
                    int picTableWidth = 603;
                    int picTableHeight = 342;

                    Ball mirroredBall = new Ball("m1", (int)(ballOn.X - Ball.Radius), (int)(-1.0 * ballOn.Y), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball("m2", (int)(-1.0 * ballOn.X), (int)(ballOn.Y), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball("m3", (int)(ballOn.X), (int)(ballOn.Y + (picTableHeight * 2.0)), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball("m4", (int)(ballOn.X + (picTableWidth * 2.0)), (int)(ballOn.Y), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                }
            }

            int ghostBallCount = ghostBalls.Count;
            if (ghostBallCount > 0)
            {
                Random rnd = new Random(DateTime.Now.Second);
                int index = rnd.Next(ghostBallCount);

                randomGhostBall = ghostBalls[index];
            }
            return randomGhostBall;
        }

        private List<Ball> GetGhostBalls(Ball ballOn, bool despair)
        {
            List<Ball> ghostBalls = new List<Ball>();

            int i = 0;
            foreach (Pocket pocket in _pocketManager.Pockets)
            {
                // Distances between pocket and BallOn center
                double dxPocketBallOn = pocket.HotSpotX - ballOn.X;
                double dyPocketBallOn = pocket.HotSpotY - ballOn.Y;
                double hPocketBallOn = Math.Sqrt(dxPocketBallOn * dxPocketBallOn + dyPocketBallOn * dyPocketBallOn);
                double a = dyPocketBallOn / dxPocketBallOn;

                // Distances between BallOn center and GhostBall center
                double hBallOnGhost = (Ball.Radius - 1.5) * 2.0;
                double dxBallOnGhost = hBallOnGhost * (dxPocketBallOn / hPocketBallOn);
                double dyBallOnGhost = hBallOnGhost * (dyPocketBallOn / hPocketBallOn);

                // GhostBall coordinates
                double gX = ballOn.X - dxBallOnGhost;
                double gY = ballOn.Y - dyBallOnGhost;
                double dxGhostCue = _ballManager.CueBall.X - gX;
                double dyGhostCue = _ballManager.CueBall.Y - gY;
                double hGhostCue = Math.Sqrt(dxGhostCue * dxGhostCue + dyGhostCue * dyGhostCue);

                // Distances between BallOn center and CueBall center
                double dxBallOnCueBall = ballOn.X - _ballManager.CueBall.X;
                double dyBallOnCueBall = ballOn.Y - _ballManager.CueBall.Y;
                double hBallOnCueBall = Math.Sqrt(dxBallOnCueBall * dxBallOnCueBall + dyBallOnCueBall * dyBallOnCueBall);

                //discards difficult GhostBalls
                if (despair || (Math.Sign(dxPocketBallOn) == Math.Sign(dxBallOnCueBall) && Math.Sign(dyPocketBallOn) == Math.Sign(dyBallOnCueBall)))
                {
                    Ball ghostBall = new Ball(i.ToString(), (int)gX, (int)gY, null, 0);
                    ghostBalls.Add(ghostBall);
                    i++;
                }
            }

            return ghostBalls;
        }

        private int GetRandomStrenght()
        {
            return _random.Next(20) + 30;
        }
    }
}
