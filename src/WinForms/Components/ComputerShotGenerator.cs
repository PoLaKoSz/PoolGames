using CSharpSnooker.WinForms.Components.PoolTypes;
using CSharpSnookerCore.Models;
using System;
using System.Collections.Generic;

namespace CSharpSnooker.WinForms.Components
{
    class ComputerShotGenerator
    {
        private readonly BallManager _ballManager;
        private readonly PocketManager _pocketManager;
        private readonly IPoolType _pooltype;

        private PlayerManager _playerManager;
        private Simulator _simulator;

        private static readonly Random _random;
        private static readonly int _maxAttempts;



        static ComputerShotGenerator()
        {
            _random = new Random();
            _maxAttempts = 2;
        }

        public ComputerShotGenerator(BallManager ballManager, PocketManager pocketManager, IPoolType poolType)
        {
            _ballManager = ballManager;
            _pocketManager = pocketManager;
            _pooltype = poolType;
        }



        public Vector2D GenerateComputerShot(PlayerManager playerManager, Simulator simulator)
        {
            _playerManager = playerManager;
            _simulator = simulator;

            List<Ball> auxBalls = new List<Ball>();

            foreach (Ball b in _ballManager.Balls)
            {
                Ball auxBall = new Ball(b.Id, (int)b.Position.X, (int)b.Position.Y, b.Image, b.Points)
                {
                    IsInPocket = b.IsInPocket
                };
                auxBalls.Add(auxBall);
            }

            int lastPlayerScore = _playerManager.CurrentPlayer.Points;
            int lastOpponentScore = _playerManager.OtherPlayer.Points;

            int newPlayerScore = -1;
            int newOpponentScore = 1000;

            int attemptsToWin = 0;
            int attemptsNotToLose = 0;
            int attemptsOfDespair = 0;
            while (true)
            {
                if (attemptsToWin < _maxAttempts)
                {
                    attemptsToWin++;
                }
                else if (attemptsNotToLose < _maxAttempts)
                {
                    attemptsNotToLose++;
                }
                else
                {
                    attemptsOfDespair++;
                }

                _playerManager.CurrentPlayer.Points = lastPlayerScore;
                _playerManager.OtherPlayer.Points = lastOpponentScore;

                bool despair = (attemptsOfDespair >= _maxAttempts);
                GenerateRandomTestComputerShot(despair);

                newPlayerScore = _playerManager.CurrentPlayer.Points;
                newOpponentScore = _playerManager.OtherPlayer.Points;

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
                    newOpponentScore == lastOpponentScore && (attemptsToWin >= _maxAttempts) ||
                    attemptsOfDespair > 5)
                {
                    break;
                }
            }

            _playerManager.CurrentPlayer.Points = lastPlayerScore;
            _playerManager.OtherPlayer.Points = lastOpponentScore;

            return _playerManager.CurrentPlayer.TestPosition;
        }


        private void GenerateRandomTestComputerShot(bool despair)
        {
            Ball ghostBall = null;

            List<Ball> ballOnList = _pooltype.PottableBalls(_playerManager.CurrentPlayer);

            ghostBall = GetRandomGhostBall(ballOnList, despair);

            if (ghostBall == null)
                ghostBall = GetNextBallOn();

            _playerManager.CurrentPlayer.Strength = GetRandomStrenght();

            if (ghostBall != null)
            {
                _playerManager.CurrentPlayer.TestPosition = new Vector2D((int)ghostBall.X, (int)ghostBall.Y);
                _playerManager.CurrentPlayer.TestStrength = _playerManager.CurrentPlayer.Strength;

                _simulator.ReceiveShot(_ballManager, _playerManager.CurrentPlayer);
            }
        }

        private int GetRandomStrenght()
        {
            return 45 + _random.Next(15);
        }

        private Ball GetNextBallOn()
        {
            var balls = _pooltype.PottableBalls(_playerManager.CurrentPlayer);

            return balls[_random.Next(0, balls.Count)];
        }

        private Ball GetRandomGhostBall(List<Ball> ballOnList, bool despair)
        {
            Ball randomGhostBall = null;

            List<Ball> ghostBalls = new List<Ball>();

            foreach (Ball ballOn in ballOnList)
            {
                if (!despair)
                {
                    foreach (Ball ghostBall in GetGhostBalls(ballOn, false))
                    {
                        ghostBalls.Add(ghostBall);
                    }
                }
                else
                {
                    int tableHeight = 340;
                    int tableWidth = 600;

                    Ball mirroredBall = new Ball(null, (int)(ballOn.X - Ball.Radius), (int)(-1.0 * ballOn.Y), null, ballOn.Points);
                    List<Ball> tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball(null, (int)(-1.0 * ballOn.X), (int)(ballOn.Y), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball(null, (int)(ballOn.X), (int)(ballOn.Y + (tableHeight * 2.0)), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball(null, (int)(ballOn.X + (tableWidth * 2.0)), (int)(ballOn.Y), null, ballOn.Points);
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
            var ghostBalls = new List<Ball>();

            int i = _ballManager.Balls[_ballManager.Balls.Count - 1].Points + 1;

            foreach (Pocket pocket in _pocketManager.Pockets)
            {
                // Distance between Pocket center coordinates and BallOn center coordinates
                double dxPocketBallOn = pocket.HotSpotX - ballOn.X;
                double dyPocketBallOn = pocket.HotSpotY - ballOn.Y;
                double hPocketBallOn = Math.Sqrt(dxPocketBallOn * dxPocketBallOn + dyPocketBallOn * dyPocketBallOn);
                double a = dyPocketBallOn / dxPocketBallOn;

                // Distance between BallOn center coordinates and GhostBall center coordinates
                double hBallOnGhost = (Ball.Radius - 1.5) * 2.0;
                double dxBallOnGhost = hBallOnGhost * (dxPocketBallOn / hPocketBallOn);
                double dyBallOnGhost = hBallOnGhost * (dyPocketBallOn / hPocketBallOn);

                // GhostBall coordinates
                double gX = ballOn.X - dxBallOnGhost;
                double gY = ballOn.Y - dyBallOnGhost;
                double dxGhostCue = _ballManager.CueBall.X - gX;
                double dyGhostCue = _ballManager.CueBall.Y - gY;
                double hGhostCue = Math.Sqrt(dxGhostCue * dxGhostCue + dyGhostCue * dyGhostCue);

                // Distance between BallOn center coordinates and CueBall center coordinates
                double dxBallOnCueBall = ballOn.X - _ballManager.CueBall.X;
                double dyBallOnCueBall = ballOn.Y - _ballManager.CueBall.Y;
                double hBallOnCueBall = Math.Sqrt(dxBallOnCueBall * dxBallOnCueBall + dyBallOnCueBall * dyBallOnCueBall);

                // Discards difficult GhostBalls
                if (despair || (Math.Sign(dxPocketBallOn) == Math.Sign(dxBallOnCueBall) && Math.Sign(dyPocketBallOn) == Math.Sign(dyBallOnCueBall)))
                {
                    Ball ghostBall = new Ball(i.ToString(), (int)gX, (int)gY, null, 0);
                    ghostBalls.Add(ghostBall);
                    i++;
                }
            }

            return ghostBalls;
        }
    }
}
