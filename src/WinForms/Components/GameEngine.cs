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



        public GameEngine()
        {
            _soundManager = new SoundManager();

            _ballManager = new BallManager();
            _ballManager.Load();

            _pocketManager = new PocketManager();
            _pocketManager.OnPotting += OnBallPotting;

            _playerManager = new PlayerManager(
                new ComputerViewModel(new Player(1, "Computer", isComputer: true)
                {
                    BallOn = _ballManager.GetRandomRedBall()
                }, _ballManager, _pocketManager),
                new PlayerViewModel(new Player(2, "Human")));

            View = new MainForm(this, _playerManager)
            {
                BallManager = _ballManager
            };

            _snapShotGenerator = new SnapShotGenerator(View.picTable, _soundManager);

            _borderManager = new BorderManager();
            _borderManager.OnCollision += OnBorderCollision;

            _collisionManager = new CollisionManager();
            _collisionManager.OnBallsCollision += OnBallsCollision;

            _simulator = new Simulator(_snapShotGenerator, _pocketManager, _borderManager, _collisionManager);

            ClearSounds();

            View.InitializePoolTable();
            foreach (Ball ball in _ballManager.Balls)
            {
                _snapShotGenerator.AddToCurrentFrame(ball);
            }
            _snapShotGenerator.NextFrame();
            _snapShotGenerator.PlayLastShot();

            _playerManager.CurrentPlayer.BallOn = _ballManager.GetRandomRedBall();

            SetBallOnImage();
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

            if (soundIntensity < 1)
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

            //_playerManager.CurrentPlayerVM.HitBall(new Vector2D(x, y), cueBall, View.CueBallSpinVector);

            _simulator.ReceiveShot(_ballManager, _playerManager.CurrentPlayer);

            _snapShotGenerator.PlayLastShot();

            ProcessFallenBalls();

            SetBallOnImage();

            Vector2D position = _playerManager.CurrentPlayerVM.Hitting();

            if (position != null)
                HitBall((int)position.X, (int)position.Y);
        }

        private void SetCueBallVelocity(int x, int y)
        {
            // 20 is the maximum velocity
            double v = 20 * (_playerManager.CurrentPlayer.Strength / 100.0);

            // Calculates the cue angle, and the translate velocity (normal velocity)
            double deltaX = x - _ballManager.CueBall.X;
            double deltaY = y - _ballManager.CueBall.Y;

            double h = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            double sin = deltaY / h;
            double cos = deltaX / h;

            _ballManager.CueBall.IsInPocket = false;
            _ballManager.CueBall.TranslateVelocity.X = v * cos;
            _ballManager.CueBall.TranslateVelocity.Y = v * sin;

            Vector2D normalVelocity = _ballManager.CueBall.TranslateVelocity.Normalize();

            // Calculates the top spin/back spin velocity, in the same direction as the normal velocity, but in opposite angle
            double topBottomVelocityRatio = _ballManager.CueBall.TranslateVelocity.Lenght() * (View.CueBallSpinVector.Y / 100.0);
            _ballManager.CueBall.VSpinVelocity = new Vector2D(-1.0d * topBottomVelocityRatio * normalVelocity.X, -1.0d * topBottomVelocityRatio * normalVelocity.Y);

            _playerManager.CurrentPlayer.ShotCount++;
        }

        private void ProcessFallenBalls()
        {
            _playerManager.CurrentPlayer.FoulList.Clear();

            int redCount = 0;
            int fallenRedCount = 0;
            int availableRedCount = 0;
            int wonPoints = 0;
            int lostPoints = 0;
            bool someInTable = false;

            foreach (Ball ball in _ballManager.Balls)
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

            foreach (Ball ball in _ballManager.PottedBalls)
            {
                if (ball.Points == 0)
                {
                    ball.ResetPositionAt(ball.InitPosition.X, ball.InitPosition.Y);
                    ball.IsInPocket = false;
                }
                else if (ball.Points > 1)
                {
                    int ballOnPoints = 1;
                    if (_playerManager.CurrentPlayer.BallOn != null)
                    {
                        ballOnPoints = _playerManager.CurrentPlayer.BallOn.Points;
                    }

                    if (fallenRedCount < redCount || ballOnPoints != ball.Points)
                    {
                        for (int points = ball.Points; points > 1; points--)
                        {
                            Ball candidateBall = _ballManager.GetCandidateBall(ball, points);
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

            foreach (Ball ball in _ballManager.StrokenBalls)
            {
                // Causing the cue ball to first hit a ball other than the ball on
                if (strokenBallsCount == 0 && ball.Points != _playerManager.CurrentPlayer.BallOn.Points)
                {
                    _playerManager.CurrentPlayer.FoulList.Add((_playerManager.CurrentPlayer.BallOn.Points < 4 ? 4 : _playerManager.CurrentPlayer.BallOn.Points));
                }

                strokenBallsCount++;
            }

            // Foul: causing the cue ball to miss all object balls
            if (strokenBallsCount == 0)
            {
                _playerManager.CurrentPlayer.FoulList.Add(4);
            }

            foreach (Ball ball in _ballManager.PottedBalls)
            {
                // Causing the cue ball to enter a pocket
                if (ball.Points == 0)
                {
                    _playerManager.CurrentPlayer.FoulList.Add(4);
                }

                // Causing: this is not the BallOn in the Pocket
                if (ball.Points != _playerManager.CurrentPlayer.BallOn.Points)
                {
                    _playerManager.CurrentPlayer.FoulList.Add(_playerManager.CurrentPlayer.BallOn.Points < 4 ? 4 : _playerManager.CurrentPlayer.BallOn.Points);
                }
            }

            if (_playerManager.CurrentPlayer.FoulList.Count == 0)
            {
                foreach (Ball ball in _ballManager.PottedBalls)
                {
                    // Legally potting reds or colors
                    wonPoints += ball.Points;
                }
            }
            else
            {
                _playerManager.CurrentPlayer.FoulList.Sort();
                lostPoints = _playerManager.CurrentPlayer.FoulList[_playerManager.CurrentPlayer.FoulList.Count - 1];
            }

            if (wonPoints == 0 || lostPoints > 0)
            {
                ChooseNextBallOn(availableRedCount, isLostBreak: true);

                _playerManager.CurrentPlayer.Points -= lostPoints;
                _playerManager.OtherPlayer.Points += lostPoints;

                _playerManager.Switch();
            }
            else
            {
                ChooseNextBallOn(availableRedCount, isLostBreak: false);

                _playerManager.CurrentPlayer.Points += wonPoints;
            }

            if (!someInTable)
            {
                throw new NotImplementedException("Somebody won!");
            }

            int fallenBallsCount = _ballManager.FallenBalls.Count;
            for (int i = fallenBallsCount - 1; i >= 0; i--)
            {
                if (!_ballManager.FallenBalls[i].IsInPocket)
                {
                    _ballManager.FallenBalls.RemoveAt(i);
                }
            }

            View.ResetCueBallSpinIndicator();

            _ballManager.StrokenBalls.Clear();
            _ballManager.PottedBalls.Clear();
            _soundManager.Empty();

            RefreshTable();

            ShowPoints();
        }

        private void ChooseNextBallOn(int availableRedCount, bool isLostBreak)
        {
            // Ha van még piros labra
            //      - és az előző BallOn nem piros volt
            //      - és az előző BallOn piros volt
            // Ha nincs piros labda, akkor a legkisebb színes

            if (isLostBreak && 0 < availableRedCount)
            {
                _playerManager.CurrentPlayer.BallOn = _ballManager.GetRandomRedBall();
            }
            else if (isLostBreak && 0 == availableRedCount)
            {
                _playerManager.CurrentPlayer.BallOn = _ballManager.GetMinColouredball();
            }
            else if (0 < availableRedCount && _playerManager.CurrentPlayer.BallOn.Points != 1)
            {
                _playerManager.CurrentPlayer.BallOn = _ballManager.GetRandomRedBall();
            }
            else if (0 < availableRedCount && _playerManager.CurrentPlayer.BallOn.Points == 1)
            {
                if (!_playerManager.CurrentPlayer.IsComputer)
                    _playerManager.CurrentPlayer.BallOn = null;
                else
                    _playerManager.CurrentPlayer.BallOn = _ballManager.GetMinColouredball();
            }
            else if (0 == availableRedCount)
            {
                _playerManager.CurrentPlayer.BallOn = _ballManager.GetMinColouredball();
            }
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
    }
}
