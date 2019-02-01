using CSharpSnookerCore.Models;
using System;

namespace CSharpSnooker.WinForms.Components
{
    class Simulator
    {
        private int moveCount;
        public PoolState PoolState { get; private set; }
        private BallManager _ballManager;
        private Player _player;

        private readonly SnapShotGenerator _snapShotGenerator;
        private readonly PocketManager _pocketManager;
        private readonly BorderManager _borderManager;
        private readonly CollisionManager _collisionManager;

        private static readonly double _friction = 0.0075d;



        static Simulator()
        {
            _friction = 0.0075F;
        }

        public Simulator(SnapShotGenerator snapShotGenerator, PocketManager pocketManager, BorderManager borderManager, CollisionManager collisionManager)
        {
            PoolState = PoolState.AwaitingShot;

            _snapShotGenerator = snapShotGenerator;
            _pocketManager = pocketManager;
            _borderManager = borderManager;
            _collisionManager = collisionManager;
        }



        public void ReceiveShot(BallManager ballManager, Player player)
        {
            PoolState = PoolState.Moving;
            _ballManager = ballManager;
            _player = player;

            while (PoolState == PoolState.Moving)
                MoveBalls();
        }


        private void MoveBalls()
        {
            bool conflicted = true;

            while (conflicted)
            {
                conflicted = false;

                bool someCollision = true;
                while (someCollision)
                {
                    foreach (Ball ball in _ballManager.Balls)
                    {
                        foreach (Pocket pocket in _pocketManager.Pockets)
                        {
                            _pocketManager.DetectPotting(ball, pocket);
                        }
                    }

                    someCollision = false;
                    foreach (Ball ballA in _ballManager.Balls)
                    {
                        if (ballA.IsInPocket)
                        {
                            ballA.Velocity.X =
                            ballA.Velocity.Y = 0.0;
                        }

                        foreach (DiagonalBorder diagonalBorder in _borderManager.DiagonalBorders)
                        {
                            if (_borderManager.CheckCollision(ballA, diagonalBorder) && !ballA.IsInPocket)
                            {
                                _collisionManager.ResolveCollision(ballA, diagonalBorder);
                            }
                        }

                        RectangleCollision borderCollision = RectangleCollision.None;
                        foreach (TableBorder tableBorder in _borderManager.TableBorders)
                        {
                            borderCollision = _borderManager.CheckCollision(ballA, tableBorder);

                            if (borderCollision != RectangleCollision.None && !ballA.IsInPocket)
                            {
                                someCollision = true;
                                _collisionManager.ResolveCollision(ballA, tableBorder, borderCollision);
                            }
                        }

                        foreach (Ball ballB in _ballManager.Balls)
                        {
                            if (ballA.Id.CompareTo(ballB.Id) != 0)
                            {
                                if (_ballManager.AreColliding(ballA, ballB) && !ballA.IsInPocket && !ballB.IsInPocket)
                                {
                                    if (ballA.Points == 0)
                                    {
                                        _ballManager.StrokenBalls.Add(ballB);
                                    }
                                    else if (ballB.Points == 0)
                                    {
                                        _ballManager.StrokenBalls.Add(ballA);
                                    }

                                    while (_ballManager.AreColliding(ballA, ballB))
                                    {
                                        someCollision = true;
                                        _collisionManager.ResolveCollision(ballA, ballB);
                                    }
                                }
                            }
                        }

                        if (ballA.IsInPocket)
                        {
                            ballA.Velocity.X =
                                ballA.Velocity.Y =
                                ballA.VSpinVelocity.X =
                                ballA.VSpinVelocity.Y = 0.0d;
                        }

                        if (ballA.Velocity.X != 0.0d ||
                            ballA.Velocity.Y != 0.0d)
                        {

                            double signalXVelocity = ballA.Velocity.X >= 0 ? 1.0 : -1.0;
                            double signalYVelocity = ballA.Velocity.Y >= 0 ? 1.0 : -1.0;

                            double absXVelocity = Math.Abs(ballA.Velocity.X);
                            double absYVelocity = Math.Abs(ballA.Velocity.Y);

                            Vector2D absVelocity = new Vector2D(absXVelocity, absYVelocity);

                            Vector2D normalizedDiff = absVelocity.Normalize();

                            absVelocity.X = absVelocity.X * (1.0 - _friction) - normalizedDiff.X * _friction;
                            absVelocity.Y = absVelocity.Y * (1.0 - _friction) - normalizedDiff.Y * _friction;

                            if (absVelocity.X < 0d)
                                absVelocity.X = 0d;

                            if (absVelocity.Y < 0d)
                                absVelocity.Y = 0d;

                            double vx = absVelocity.X * signalXVelocity;
                            double vy = absVelocity.Y * signalYVelocity;

                            if (double.IsNaN(vx))
                                vx = 0;

                            if (double.IsNaN(vy))
                                vy = 0;

                            ballA.Velocity = new Vector2D(vx, vy);
                        }

                        if (ballA.VSpinVelocity.X != 0.0d || ballA.VSpinVelocity.Y != 0.0d)
                        {
                            double signalXVelocity = ballA.VSpinVelocity.X >= 0 ? 1.0 : -1.0;
                            double signalYVelocity = ballA.VSpinVelocity.Y >= 0 ? 1.0 : -1.0;
                            double absXVelocity = Math.Abs(ballA.VSpinVelocity.X);
                            double absYVelocity = Math.Abs(ballA.VSpinVelocity.Y);

                            Vector2D absVelocity = new Vector2D(absXVelocity, absYVelocity);

                            Vector2D normalizedDiff = absVelocity.Normalize();

                            absVelocity.X = absVelocity.X - normalizedDiff.X * _friction / 1.2;
                            absVelocity.Y = absVelocity.Y - normalizedDiff.Y * _friction / 1.2;

                            if (absVelocity.X < 0d)
                                absVelocity.X = 0d;

                            if (absVelocity.Y < 0d)
                                absVelocity.Y = 0d;

                            ballA.VSpinVelocity = new Vector2D(absVelocity.X * signalXVelocity, absVelocity.Y * signalYVelocity);
                        }
                    }

                    foreach (Ball ball in _ballManager.Balls)
                    {
                        ball.Position.X += ball.Velocity.X + ball.VSpinVelocity.X;
                        ball.Position.Y += ball.Velocity.Y + ball.VSpinVelocity.Y;
                    }
                }

                MoveBall(false);
                conflicted = false;
            }

            double totalVelocity = 0;
            foreach (Ball ball in _ballManager.Balls)
            {
                totalVelocity += ball.Velocity.X;
                totalVelocity += ball.Velocity.Y;
            }

            if (PoolState == PoolState.Moving && totalVelocity == 0)
            {
                if (PoolState == PoolState.Moving)
                {
                    PoolState = PoolState.AwaitingShot;
                    return;
                }
            }
        }

        private void MoveBall(bool forcePaint)
        {
            moveCount++;

            if (moveCount < _snapShotGenerator.VideoRefreshRate && !forcePaint)
            {
                return;
            }
            else
            {
                moveCount = 0;
            }

            bool someMoved = false;

            foreach (Ball ball in _ballManager.Balls)
            {
                if (!ball.IsInPocket)
                {
                    if (forcePaint || ((int)ball.X != (int)ball.LastX || (int)ball.Y != (int)ball.LastY))
                    {
                        someMoved = true;
                        break;
                    }
                }
            }

            if (someMoved || forcePaint)
            {
                foreach (Ball ball in _ballManager.Balls)
                {
                    int lastX = (int)ball.LastX;
                    int X = (int)ball.X;
                    int lastY = (int)ball.LastY;
                    int Y = (int)ball.Y;

                    if (!ball.IsInPocket)
                    {
                        _snapShotGenerator.AddToCurrentFrame(ball);

                        ball.LastX = ball.X;
                        ball.LastY = ball.Y;
                    }
                }

                _snapShotGenerator.NextFrame();
            }
        }
    }
}
