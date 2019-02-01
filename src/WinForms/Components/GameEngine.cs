using CSharpSnooker.WinForms.Components.PoolTypes;
using CSharpSnooker.WinForms.Models.Events;
using CSharpSnookerCore.Models;
using CSharpSnookerCore.Models.Sounds;
using System;
using System.Windows.Forms;

namespace CSharpSnooker.WinForms.Components
{
    public enum PoolState
    {
        AwaitingShot,
        Moving
    }

    class GameEngine
    {
        public MainForm View { get; }

        private readonly PlayerManager _playerManager;
        private readonly SoundManager _soundManager;
        private readonly SnapShotGenerator _snapShotGenerator;
        private readonly BallManager _ballManager;
        private readonly PocketManager _pocketManager;
        private readonly BorderManager _borderManager;
        private readonly Simulator _simulator;
        private readonly CollisionManager _collisionManager;
        private readonly IPoolType _poolType;



        public GameEngine()
        {
            _soundManager = new SoundManager();

            _poolType = new SnookerPool();

            _ballManager = new BallManager();
            _ballManager.Load(_poolType.Balls);

            _pocketManager = new PocketManager();

            _playerManager = new PlayerManager("Human", _ballManager, new ComputerShotGenerator(_ballManager, _pocketManager, _poolType));

            _poolType.InitBallOn(_playerManager, _ballManager);

            View = new MainForm(this, _playerManager);

            _snapShotGenerator = new SnapShotGenerator(View.picTable, _soundManager);

            _borderManager = new BorderManager();

            _collisionManager = new CollisionManager();

            _simulator = new Simulator(_snapShotGenerator, _pocketManager, _borderManager, _collisionManager);

            SubScribeEvents();

            ClearSounds();

            View.InitializePoolTable();
            foreach (Ball ball in _ballManager.Balls)
            {
                _snapShotGenerator.AddToCurrentFrame(ball);
            }
            _snapShotGenerator.NextFrame();
            _snapShotGenerator.PlayLastShot();

            SetBallOnImage();
        }



        private void SubScribeEvents()
        {
            _pocketManager.OnPotting += OnBallPotting;
            _borderManager.OnCollision += OnBorderCollision;
            _collisionManager.OnBallsCollision += OnBallsCollision;
        }

        private void UnSubScribeEvents()
        {
            _pocketManager.OnPotting -= OnBallPotting;
            _borderManager.OnCollision -= OnBorderCollision;
            _collisionManager.OnBallsCollision -= OnBallsCollision;
        }

        private void OnBallPotting(BallPottedEventArgs e)
        {
            _soundManager.Add(_snapShotGenerator.SnapShotCount, new FallSound(e.Ball));

            _ballManager.FallenBalls.Add(e.Ball);
            _ballManager.PottedBalls.Add(e.Ball);
        }

        private void OnBorderCollision(BorderCollisionEventArgs e)
        {
            _soundManager.Add(_snapShotGenerator.SnapShotCount, new BankSound(e.Ball));
        }

        private void OnBallsCollision(BallsCollisionEventArgs e)
        {
            int soundIntensity = (int)(Math.Abs(e.Impulse.X) + Math.Abs(e.Impulse.Y));

            if (soundIntensity > 5)
            {
                soundIntensity = 5;
            }
            else if (soundIntensity < 1)
            {
                soundIntensity = 1;
            }

            _soundManager.Add(_snapShotGenerator.SnapShotCount, new HitSound(soundIntensity.ToString("00"), e.Ball));
        }

        private void ClearSounds()
        {
            _soundManager.Empty();

            for (int i = 0; i < 300; i++)
            {
                _soundManager.Add();
            }
        }

        private void HitBall(int x, int y)
        {
            _snapShotGenerator.DeleteSnapShots();

            View.HittingBall();

            _soundManager.Add(_snapShotGenerator.SnapShotCount, new ShotSound(_ballManager.CueBall));

            SetCueBallVelocity(x, y);

            _simulator.ReceiveShot(_ballManager, _playerManager.CurrentPlayer);

            _snapShotGenerator.PlayLastShot();

            _poolType.ProcessFallenBalls(_ballManager, _playerManager);

            if (_poolType.HasWinner)
            {
                View.ShowWinner();

                _playerManager.EndMatch();
                return;
            }

            View.ResetCueBallSpinIndicator();

            _soundManager.Empty();

            RefreshTable();

            ShowPoints();

            SetBallOnImage();

            if (_playerManager.CurrentPlayer.IsComputer)
            {
                UnSubScribeEvents();

                var shotVector = _playerManager.CurrentPlayerVM.GiveControl(_playerManager, _simulator);

                SubScribeEvents();

                HitBall((int)shotVector.X, (int)shotVector.Y);
            }
        }

        private void SetCueBallVelocity(int x, int y)
        {
            // 20 is the maximum velocity
            double v = 20 * (_playerManager.CurrentPlayer.Strength / 100.0);

            // Calculates the Cue angle, and the translate velocity (normal velocity)
            double deltaX = x - _ballManager.CueBall.X;
            double deltaY = y - _ballManager.CueBall.Y;

            double h = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            double sin = deltaY / h;
            double cos = deltaX / h;

            _ballManager.CueBall.IsInPocket = false;
            _ballManager.CueBall.Velocity.X = v * cos;
            _ballManager.CueBall.Velocity.Y = v * sin;

            Vector2D normalVelocity = _ballManager.CueBall.Velocity.Normalize();

            // Calculates the top spin/back spin velocity, in the same direction as the normal velocity, but in opposite angle
            double topBottomVelocityRatio = _ballManager.CueBall.Velocity.Lenght() * (View.CueBallSpinVector.Y / 100.0);
            _ballManager.CueBall.VSpinVelocity = new Vector2D(-1.0d * topBottomVelocityRatio * normalVelocity.X, -1.0d * topBottomVelocityRatio * normalVelocity.Y);
        }

        private void RefreshTable()
        {
            _snapShotGenerator.DeleteSnapShots();

            foreach (var ball in _ballManager.Balls)
            {
                if (!ball.IsInPocket)
                    _snapShotGenerator.AddToCurrentFrame(ball);
            }

            _snapShotGenerator.PlayLastShot();
        }

        private void ShowPoints()
        {
            if (_playerManager.CurrentPlayer.IsComputer)
            {
                View.ShowPoints(_playerManager.CurrentPlayer, _playerManager.OtherPlayer);
            }
            else
            {
                View.ShowPoints(_playerManager.OtherPlayer, _playerManager.CurrentPlayer);
            }
        }

        public void PoolTable_MouseUp(MouseEventArgs e)
        {
            if (!_playerManager.CurrentPlayer.IsComputer)
            {
                if (_playerManager.CurrentPlayer.BallOn == null)
                {
                    Ball bOn = _ballManager.GetBallOn(e.X, e.Y);

                    if (bOn != null)
                    {
                        _playerManager.CurrentPlayer.BallOn = bOn;
                        View.AimWithCueBall();
                    }
                }
                else
                    HitBall(e.X, e.Y);
            }
        }

        public void PoolTable_MouseMove(MouseEventArgs e)
        {
            if (!_playerManager.CurrentPlayer.IsComputer)
            {
                if (_playerManager.CurrentPlayer.BallOn == null)
                {
                    Ball ballOn = _ballManager.GetBallOn(e.X, e.Y);

                    if (ballOn != null)
                    {
                        View.CallingBall();
                    }
                    else
                    {
                        View.AimWithCueBall();
                    }
                }
                else
                {
                    View.AimWithCueBall();
                }
            }
        }

        private void SetBallOnImage()
        {
            View.SetBallOnImage(_playerManager.CurrentPlayer.BallOn);
        }

        ~GameEngine()
        {
            UnSubScribeEvents();
        }
    }
}
