using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CSharpSnooker.WinForms.Components;
using CSharpSnookerCore.Models;
using CSharpSnookerCore.Models.Sounds;

namespace CSharpSnooker.WinForms
{
    public enum PoolState
    {
        AwaitingShot,
        Moving
    }

    public enum PlayerState
    {
        None,
        Aiming,
        Calling,
        GameOver
    }

    public partial class MainForm : Form, IBallObserver, IPocketObserver, IBorderObserver
    {
        private readonly PlayerManager _playerManager;
        private readonly SoundManager _soundManager;
        private readonly SnapShotGenerator _snapShotGenerator;
        private readonly BallManager _ballManager;


        bool showBallOn = true;
        
        const int MAX_COMPUTER_ATTEMPTS = 2;
        Vector2D targetVector = new Vector2D(0, 0);

        Rectangle thermometerRectangle = new Rectangle(32, 249, 152, 10);
        Rectangle targetRectangle = new Rectangle(68, 269, 71, 71);

        PoolState poolState = PoolState.AwaitingShot;
        Single friction = 0.0075F;
        List<Pocket> pockets = new List<Pocket>();
        List<TableBorder> tableBorders = new List<TableBorder>();
        List<DiagonalBorder> diagonalBorders = new List<DiagonalBorder>();
        
        Image imgQuestionBall;
        Image imgShadow;
        Graphics tableGraphics;
        int moveCount = 0;
        PlayerState playerState = PlayerState.None;



        public MainForm()
        {
            _playerManager = new PlayerManager(new Player(1, "Computer", isComputer: true), new Player(2, "Human"));
            _soundManager = new SoundManager();

            InitializeComponent();

            _ballManager = new BallManager(this);
            _snapShotGenerator = new SnapShotGenerator(picTable, _ballManager.Balls, _soundManager);

            lblPlayer1Name.Text = _playerManager.CurrentPlayer.Name;
            lblPlayer2Name.Text = _playerManager.OtherPlayer.Name;

            ClearFramesAndSounds();

            imgShadow = Image.FromFile(@"Images\ShadowBall.PNG");
            imgQuestionBall = Image.FromFile(@"Images\questionball.PNG");


            pockets.Add(new Pocket(this, 1, 5, 5, 29, 29));
            pockets.Add(new Pocket(this, 2, 288, 0, 301, 25));
            pockets.Add(new Pocket(this, 3, 571, 5, 573, 29));
            pockets.Add(new Pocket(this, 4, 5, 309, 29, 309));
            pockets.Add(new Pocket(this, 5, 288, 314, 301, 313));
            pockets.Add(new Pocket(this, 6, 571, 309, 572, 310));
            _ballManager.Load(this);

            diagonalBorders.Add(new DiagonalBorder(547, 309, 35, Side.Southwest));
            diagonalBorders.Add(new DiagonalBorder(573, 286, 35, Side.Northeast));
            diagonalBorders.Add(new DiagonalBorder(1, 27, 35, Side.Southwest));
            diagonalBorders.Add(new DiagonalBorder(24, 1, 35, Side.Northeast));
            diagonalBorders.Add(new DiagonalBorder(546, 33, 35, Side.Northwest));
            diagonalBorders.Add(new DiagonalBorder(567, 59, 35, Side.Southeast));
            diagonalBorders.Add(new DiagonalBorder(1, 319, 35, Side.Northwest));
            diagonalBorders.Add(new DiagonalBorder(18, 344, 35, Side.Southeast));

            tableBorders.Add(new TableBorder(this, 0, 55, 27, 235, ForcedDirection.None));
            tableBorders.Add(new TableBorder(this, 577, 55, 27, 235, ForcedDirection.None));
            tableBorders.Add(new TableBorder(this, 51, 0, 230, 27, ForcedDirection.None));
            tableBorders.Add(new TableBorder(this, 51, 316, 230, 27, ForcedDirection.None));
            tableBorders.Add(new TableBorder(this, 319, 0, 235, 27, ForcedDirection.None));
            tableBorders.Add(new TableBorder(this, 319, 316, 235, 27, ForcedDirection.None));

            tableBorders.Add(new TableBorder(this, -20, 55, 20, 289, ForcedDirection.None));
            tableBorders.Add(new TableBorder(this, 606, 55, 20, 289, ForcedDirection.None));
            tableBorders.Add(new TableBorder(this, 0, -20, 606, 20, ForcedDirection.None));
            tableBorders.Add(new TableBorder(this, 0, 344, 606, 20, ForcedDirection.None));

            lblStrenght.Width = (int)((_playerManager.CurrentPlayer.Strength * (thermometerRectangle.Width - 12) / 100.0));
            _playerManager.CurrentPlayer.BallOn = _ballManager.Balls[1];
            SetBallOnImage();

            timerInBox.Enabled = _playerManager.CurrentPlayer.IsComputer;

            UpdatePlayerState(PlayerState.Aiming);
            _playerManager.CurrentPlayer.BallOn = _ballManager.GetRandomRedBall();
            _playerManager.CurrentPlayer.Strength = GetRandomStrenght();
            SetBallOnImage();
            timerComputer.Enabled = true;
        }



        private void ClearFramesAndSounds()
        {
            _soundManager.Empty();

            for (int i = 0; i < 300; i++)
            {
                _soundManager.Add();
            }
        }

        private void timerBallOn_Tick(object sender, EventArgs e)
        {
            if (playerState == PlayerState.Aiming || playerState == PlayerState.Calling)
            {
                picBallOn.Top = 90 + (_playerManager.CurrentPlayer.Id - 1) * 58;
                showBallOn = !showBallOn;
                picBallOn.Visible = showBallOn;
            }
        }

        private void MoveBalls(bool test)
        {
            foreach (Ball ball in _ballManager.Balls)
            {
                if (Math.Abs(ball.X) < 5 && Math.Abs(ball.Y) < 5 && Math.Abs(ball.TranslateVelocity.X) < 10 && Math.Abs(ball.TranslateVelocity.Y) < 10)
                {
                    ball.X =
                    ball.Y = 0;

                    ball.TranslateVelocity.X =
                    ball.TranslateVelocity.Y = 0;
                }
            }

            bool conflicted = true;

            while (conflicted)
            {
                conflicted = false;

                bool someCollision = true;
                while (someCollision)
                {
                    foreach (Ball ball in _ballManager.Balls)
                    {
                        foreach (Pocket pocket in pockets)
                        {
                            bool inPocket = pocket.IsBallInPocket(ball);
                        }
                    }

                    someCollision = false;
                    foreach (Ball ballA in _ballManager.Balls)
                    {
                        if (ballA.IsBallInPocket)
                        {
                            ballA.TranslateVelocity.X =
                            ballA.TranslateVelocity.Y = 0.0;
                        }

                        foreach (DiagonalBorder diagonalBorder in diagonalBorders)
                        {
                            if (diagonalBorder.Colliding(ballA) && !ballA.IsBallInPocket)
                            {
                                diagonalBorder.ResolveCollision(ballA);
                            }
                        }

                        RectangleCollision borderCollision = RectangleCollision.None;
                        foreach (TableBorder tableBorder in tableBorders)
                        {
                            borderCollision = tableBorder.Colliding(ballA);

                            if (borderCollision != RectangleCollision.None && !ballA.IsBallInPocket)
                            {
                                someCollision = true;
                                tableBorder.ResolveCollision(ballA, borderCollision);
                            }
                        }

                        foreach (Ball ballB in _ballManager.Balls)
                        {
                            if (ballA.Id.CompareTo(ballB.Id) != 0)
                            {
                                if (ballA.Colliding(ballB) && !ballA.IsBallInPocket && !ballB.IsBallInPocket)
                                {
                                    if (ballA.Points == 0)
                                    {
                                        _ballManager.StrokenBalls.Add(ballB);
                                    }
                                    else if (ballB.Points == 0)
                                    {
                                        _ballManager.StrokenBalls.Add(ballA);
                                    }

                                    while (ballA.Colliding(ballB))
                                    {
                                        someCollision = true;
                                        ballA.ResolveCollision(ballB);
                                    }
                                }
                            }
                        }

                        if (ballA.IsBallInPocket)
                        {
                            ballA.TranslateVelocity.X =
                            ballA.TranslateVelocity.Y =
                            ballA.VSpinVelocity.X =
                            ballA.VSpinVelocity.Y = 0.0d;
                        }

                        if (ballA.TranslateVelocity.X != 0.0d ||
                            ballA.TranslateVelocity.Y != 0.0d)
                        {

                            double signalXVelocity = ballA.TranslateVelocity.X >= 0 ? 1.0 : -1.0;
                            double signalYVelocity = ballA.TranslateVelocity.Y >= 0 ? 1.0 : -1.0;
                            double absXVelocity = Math.Abs(ballA.TranslateVelocity.X);
                            double absYVelocity = Math.Abs(ballA.TranslateVelocity.Y);

                            Vector2D absVelocity = new Vector2D(absXVelocity, absYVelocity);

                            Vector2D normalizedDiff = absVelocity.Normalize();

                            absVelocity.X = absVelocity.X * (1.0 - friction) - normalizedDiff.X * friction;
                            absVelocity.Y = absVelocity.Y * (1.0 - friction) - normalizedDiff.Y * friction;

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

                            ballA.TranslateVelocity = new Vector2D(vx, vy);
                        }

                        if (ballA.VSpinVelocity.X != 0.0d || ballA.VSpinVelocity.Y != 0.0d)
                        {

                            double signalXVelocity = ballA.VSpinVelocity.X >= 0 ? 1.0 : -1.0;
                            double signalYVelocity = ballA.VSpinVelocity.Y >= 0 ? 1.0 : -1.0;
                            double absXVelocity = Math.Abs(ballA.VSpinVelocity.X);
                            double absYVelocity = Math.Abs(ballA.VSpinVelocity.Y);

                            Vector2D absVelocity = new Vector2D(absXVelocity, absYVelocity);

                            Vector2D normalizedDiff = absVelocity.Normalize();

                            absVelocity.X = absVelocity.X - normalizedDiff.X * friction / 1.2;
                            absVelocity.Y = absVelocity.Y - normalizedDiff.Y * friction / 1.2;

                            if (absVelocity.X < 0d)
                                absVelocity.X = 0d;

                            if (absVelocity.Y < 0d)
                                absVelocity.Y = 0d;

                            ballA.VSpinVelocity = new Vector2D(absVelocity.X * signalXVelocity, absVelocity.Y * signalYVelocity);
                        }
                    }

                    foreach (Ball ball in _ballManager.Balls)
                    {
                        ball.Position.X += ball.TranslateVelocity.X + ball.VSpinVelocity.X;
                        ball.Position.Y += ball.TranslateVelocity.Y + ball.VSpinVelocity.Y;
                    }
                }

                MoveBall(false);
                conflicted = false;
            }

            double totalVelocity = 0;
            foreach (Ball ball in _ballManager.Balls)
            {
                totalVelocity += ball.TranslateVelocity.X;
                totalVelocity += ball.TranslateVelocity.Y;
            }

            if (poolState == PoolState.Moving && totalVelocity == 0)
            {
                if (poolState == PoolState.Moving)
                {
                    poolState = PoolState.AwaitingShot;
                    return;
                }
            }
        }

        private void AfterBallsGetStill(bool test)
        {
            MoveBall(true);
            _snapShotGenerator.UpdateMaxSnapshot();

            if (!test)
            {
                _snapShotGenerator.DrawSnapShots();
                _snapShotGenerator.PlayLastShot();
            }

            ProcessFallenBalls(test);

            if (playerState != PlayerState.GameOver)
            {
                _snapShotGenerator.PositionsClear();
                MoveBall(true);

                if (!test)
                {
                    _snapShotGenerator.DrawSnapShots();
                    _snapShotGenerator.PlayLastShot();
                    if (!_playerManager.CurrentPlayer.IsComputer)
                    {
                        picTable.Cursor = Cursors.AppStarting;
                    }
                    else
                    {
                        picTable.Cursor = Cursors.Arrow;
                    }
                    picTable.Cursor = Cursors.SizeAll;
                }

                _snapShotGenerator.PositionsClear();

                if (!test)
                {
                    timerInBox.Enabled = _playerManager.CurrentPlayer.IsComputer;

                    if (playerState == PlayerState.Aiming || playerState == PlayerState.Calling)
                    {
                        timerComputer.Enabled = _playerManager.CurrentPlayer.IsComputer;
                    }
                }
            }
            return;
        }

        private void DrawBorderLines()
        {
            foreach (DiagonalBorder diagonalBorder in diagonalBorders)
            {
                tableGraphics.DrawLine(new Pen(Brushes.White), diagonalBorder.X1, diagonalBorder.Y1, diagonalBorder.X2, diagonalBorder.Y2);
            }

            foreach (TableBorder tableBorder in tableBorders)
            {
                tableGraphics.DrawRectangle(new Pen(Brushes.White), tableBorder.X, tableBorder.Y, tableBorder.Width, tableBorder.Height);
            }

            foreach (Pocket pocket in pockets)
            {
                tableGraphics.DrawEllipse(new Pen(Brushes.White), pocket.X - (int)Ball.Radius, pocket.Y - (int)Ball.Radius, (int)Ball.Radius * 2, (int)Ball.Radius * 2);
            }
        }

        private void ProcessFallenBalls(bool test)
        {
            _playerManager.CurrentPlayer.FoulList.Clear();

            int redCount = 0;
            int fallenRedCount = 0;
            int wonPoints = 0;
            int lostPoints = 0;
            bool someInTable = false;

            foreach (Ball ball in _ballManager.Balls)
            {
                if (!ball.IsBallInPocket)
                {
                    if (ball.Points > 0)
                        someInTable = true;
                }

                if (ball.Points == 1)
                {
                    redCount++;
                }
            }

            foreach (Ball ball in _ballManager.Balls)
            {
                if (ball.Points == 1 && ball.IsBallInPocket)
                {
                    fallenRedCount++;
                }
            }

            foreach (Ball ball in _ballManager.PottedBalls)
            {
                if (ball.Points == 0)
                {
                    ball.ResetPositionAt(ball.InitPosition.X, ball.InitPosition.Y);
                    ball.IsBallInPocket = false;
                }
                else if (ball.Points > 1)
                {
                    int ballOnPoints = 1;
                    if (_playerManager.CurrentPlayer.BallOn != null)
                        ballOnPoints = _playerManager.CurrentPlayer.BallOn.Points;

                    if (fallenRedCount < redCount || ballOnPoints != ball.Points)
                    {
                        for (int points = ball.Points; points > 1; points--)
                        {
                            Ball candidateBall = _ballManager.GetCandidateBall(ball, points);
                            if (candidateBall != null)
                            {
                                ball.ResetPositionAt(candidateBall.InitPosition.X, candidateBall.InitPosition.Y);
                                ball.IsBallInPocket = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (_playerManager.CurrentPlayer.BallOn == null)
                _playerManager.CurrentPlayer.BallOn = _ballManager.Balls[1];

            int strokenBallsCount = 0;
            foreach (Ball ball in _ballManager.StrokenBalls)
            {
                //causing the cue ball to first hit a ball other than the ball on
                if (strokenBallsCount == 0 && ball.Points != _playerManager.CurrentPlayer.BallOn.Points)
                    _playerManager.CurrentPlayer.FoulList.Add((_playerManager.CurrentPlayer.BallOn.Points < 4 ? 4 : _playerManager.CurrentPlayer.BallOn.Points));

                strokenBallsCount++;
            }

            //Foul: causing the cue ball to miss all object balls
            if (strokenBallsCount == 0)
                _playerManager.CurrentPlayer.FoulList.Add(4);

            foreach (Ball ball in _ballManager.PottedBalls)
            {
                //causing the cue ball to enter a pocket
                if (ball.Points == 0)
                    _playerManager.CurrentPlayer.FoulList.Add(4);

                //causing a ball not on to enter a pocket
                if (ball.Points != _playerManager.CurrentPlayer.BallOn.Points)
                    _playerManager.CurrentPlayer.FoulList.Add(_playerManager.CurrentPlayer.BallOn.Points < 4 ? 4 : _playerManager.CurrentPlayer.BallOn.Points);
            }

            if (_playerManager.CurrentPlayer.FoulList.Count == 0)
            {
                foreach (Ball ball in _ballManager.PottedBalls)
                {
                    //legally potting reds or colors
                    wonPoints += ball.Points;
                }
            }
            else
            {
                _playerManager.CurrentPlayer.FoulList.Sort();
                lostPoints = _playerManager.CurrentPlayer.FoulList[_playerManager.CurrentPlayer.FoulList.Count - 1];
            }

            _playerManager.CurrentPlayer.Points += wonPoints;

            if (lostPoints > 0)
            {
                _playerManager.OtherPlayer.Points += lostPoints;
            }

            if (!test)
            {
                bool swappedPlayers = false;
                //check if it's other player's turn
                if (wonPoints == 0 || lostPoints > 0)
                {
                    swappedPlayers = true;

                    _playerManager.Switch();
                }

                ShowPoints();

                if (!someInTable)
                {
                    lblPlayer1.Invalidate();
                    lblPlayer2.Invalidate();
                    lblPlayer1.Update();
                    lblPlayer2.Update();
                    UpdatePlayerState(PlayerState.GameOver);
                    return;
                }

                int fallenBallsCount = _ballManager.FallenBalls.Count;
                for (int i = fallenBallsCount - 1; i >= 0; i--)
                {
                    if (!_ballManager.FallenBalls[i].IsBallInPocket)
                    {
                        _ballManager.FallenBalls.RemoveAt(i);
                    }
                }

                _snapShotGenerator.PositionsClear();

                _playerManager.OtherPlayer.JustSwapped = true;
                _playerManager.CurrentPlayer.JustSwapped = swappedPlayers;

                if (swappedPlayers)
                {
                    playerState = PlayerState.Aiming;
                }
                else
                {
                    if (playerState == PlayerState.Aiming)
                    {
                        if (fallenRedCount < redCount)
                        {
                            if (_playerManager.CurrentPlayer.BallOn.Points == 1)
                            {
                                playerState = PlayerState.Calling;
                                picTable.Cursor = Cursors.Hand;
                            }
                        }
                    }
                    else if (playerState == PlayerState.Calling)
                    {
                        playerState = PlayerState.Aiming;
                    }
                }
                _playerManager.CurrentPlayer.BallOn = GetNextBallOn(swappedPlayers, _playerManager.CurrentPlayer.BallOn);

                SetBallOnImage();

                int ballX = (int)(targetRectangle.X + targetRectangle.Width / 2.0);
                int ballY = (int)(targetRectangle.Y + targetRectangle.Height / 2.0);
                lblTarget.Location = new Point(ballX - 3, ballY - 3);
                targetVector = new Vector2D(0, 0);
            }

            _ballManager.StrokenBalls.Clear();
            _ballManager.PottedBalls.Clear();
            _soundManager.Empty();

            if (!_playerManager.CurrentPlayer.IsComputer)
                timerInBox.Enabled = true;
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
                if (!ball.IsBallInPocket)
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
                int index = 0;
                foreach (Ball ball in _ballManager.Balls)
                {
                    int lastX = (int)ball.LastX;
                    int X = (int)ball.X;
                    int lastY = (int)ball.LastY;
                    int Y = (int)ball.Y;

                    if (!ball.IsBallInPocket)
                    {
                        _snapShotGenerator.Add(index, ball);

                        ball.LastX = ball.X;
                        ball.LastY = ball.Y;
                    }
                    index++;
                }

                _snapShotGenerator.NextFrame();
            }
        }

        private void UpdatePlayerState(PlayerState playerState)
        {
            this.playerState = playerState;

            switch (playerState)
            {
                case PlayerState.None:
                    timerSplash.Enabled = true;
                    break;

                case PlayerState.Aiming:
                    _soundManager.StopAll();

                    if (tableGraphics == null)
                    {
                        tableGraphics = picTable.CreateGraphics();
                        MoveBall(true);
                        _snapShotGenerator.DrawSnapShots();
                        _snapShotGenerator.PlaySnapShot();
                    }
                    break;

                case PlayerState.GameOver:
                    timerComputer.Enabled = false;
                    ShowWinner();
                    UpdatePlayerState(PlayerState.Aiming);
                    break;

                default: break;
            }
        }

        private void HitBall(int x, int y, bool test)
        {
            //Reset the frames and ball positions
            _snapShotGenerator.ClearSequenceBackGround();
            _snapShotGenerator.PositionsClear();

            poolState = PoolState.Moving;
            picTable.Cursor = Cursors.WaitCursor;

            //20 is the maximum velocity
            double v = 20 * (_playerManager.CurrentPlayer.Strength / 100.0);

            //Calculates the cue angle, and the translate velocity (normal velocity)
            double dx = x - _ballManager.Balls[0].X;
            double dy = y - _ballManager.Balls[0].Y;
            double h = (double)(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
            double sin = dy / h;
            double cos = dx / h;

            _ballManager.Balls[0].IsBallInPocket = false;
            _ballManager.Balls[0].TranslateVelocity.X = v * cos;
            _ballManager.Balls[0].TranslateVelocity.Y = v * sin;
            Vector2D normalVelocity = _ballManager.Balls[0].TranslateVelocity.Normalize();

            //Calculates the top spin/back spin velocity, in the same direction as the normal velocity, but in opposite angle
            double topBottomVelocityRatio = _ballManager.Balls[0].TranslateVelocity.Lenght() * (targetVector.Y / 100.0);
            _ballManager.Balls[0].VSpinVelocity = new Vector2D(-1.0d * topBottomVelocityRatio * normalVelocity.X, -1.0d * topBottomVelocityRatio * normalVelocity.Y);
            
            _soundManager.Add(_snapShotGenerator.SnapShotCount, new ShotSound(_ballManager.Balls[0]));

            //Calculates the ball positions as long as there are moving balls
            while (poolState == PoolState.Moving)
                MoveBalls(test);

            AfterBallsGetStill(test);

            _playerManager.CurrentPlayer.ShotCount++;
        }

        private void frmTable_Deactivate(object sender, EventArgs e)
        {
            foreach (Ball ball in _ballManager.Balls)
            {
                ball.IsStill = true;
            }
        }

        private void ShowPoints()
        {
            if (_playerManager.CurrentPlayer.IsComputer)
            {
                lblPlayer1.Text = _playerManager.CurrentPlayer.Points.ToString();
                lblPlayer2.Text = _playerManager.OtherPlayer.Points.ToString();
            }
            else
            {
                lblPlayer1.Text = _playerManager.OtherPlayer.Points.ToString();
                lblPlayer2.Text = _playerManager.CurrentPlayer.Points.ToString();
            }
        }

        public void Hit(string volume, Ball ball)
        {
            _soundManager.Add(_snapShotGenerator.SnapShotCount, new HitSound(volume, ball));
        }

        public void BallDropped(Ball ball)
        {
            _soundManager.Add(_snapShotGenerator.SnapShotCount, new FallSound(ball));

            _ballManager.FallenBalls.Add(ball);
            _ballManager.PottedBalls.Add(ball);

            ball.IsBallInPocket = true;
        }

        public void WallCollision(Ball ball)
        {
            _soundManager.Add(_snapShotGenerator.SnapShotCount, new BankSound(ball));
        }

        private void picTable_MouseUp(object sender, MouseEventArgs e)
        {
            if (playerState == PlayerState.Calling || playerState == PlayerState.Aiming)
            {
                if (e.Button == MouseButtons.Left && !_playerManager.CurrentPlayer.IsComputer)
                {
                    if (playerState == PlayerState.Calling)
                    {
                        Ball bOn = _ballManager.GetBallOn(e.X, e.Y);
                        if (bOn != null)
                        {
                            _playerManager.CurrentPlayer.BallOn = bOn;
                            playerState = PlayerState.Aiming;
                            picTable.Cursor = Cursors.SizeAll;
                        }
                    }
                    else
                    {
                        if (poolState == PoolState.AwaitingShot)
                            HitBall(e.X, e.Y, false);
                    }
                }
                SetBallOnImage();
            }
        }

        private void SetBallOnImage()
        {
            if (playerState == PlayerState.Aiming)
            {
                if (_playerManager.CurrentPlayer.BallOn != null)
                    picBallOn.Image = _playerManager.CurrentPlayer.BallOn.Image;
                else
                    picBallOn.Image = null;
            }
            else
            {
                picBallOn.Image = imgQuestionBall;
            }
        }

        private void picTable_MouseMove(object sender, MouseEventArgs e)
        {
            if (playerState == PlayerState.Aiming)
            {
            }
            else if (playerState == PlayerState.Calling)
            {
                Ball bOn = null;
                int x = e.X;
                int y = e.Y;
                bOn = _ballManager.GetBallOn(x, y);
                if (bOn != null)
                {
                    picTable.Cursor = Cursors.Hand;
                }
                else
                {
                    picTable.Cursor = Cursors.Arrow;
                }
            }
        }

        private bool SetStrength(int x, int y)
        {
            bool ret = false;
            if (x >= thermometerRectangle.X && x <= thermometerRectangle.X + thermometerRectangle.Width && y >= thermometerRectangle.Y - 3 && y <= thermometerRectangle.Y + thermometerRectangle.Height + 3)
            {
                lblStrenght.Width = x - thermometerRectangle.Left - 6;

                if (!_playerManager.CurrentPlayer.IsComputer)
                {
                    _playerManager.CurrentPlayer.Strength = (int)(1.0 * lblStrenght.Width / (thermometerRectangle.Width - 12) * 100.0);
                }
                else
                {
                    _playerManager.OtherPlayer.Strength = (int)(1.0 * lblStrenght.Width / (thermometerRectangle.Width - 12) * 100.0);
                }

                ret = true;
            }
            else
                ret = false;

            return ret;
        }

        private Vector2D SetTarget(int x, int y)
        {
            Vector2D vector = targetVector;
            int ballX = (int)(targetRectangle.X + targetRectangle.Width / 2.0);
            int ballY = (int)(targetRectangle.Y + targetRectangle.Height / 2.0);
            int ballRadius = targetRectangle.Width / 2;

            float xd = (float)(x - ballX);
            float yd = (float)(y - ballY);

            float sumRadius = (float)(targetRectangle.Width / 2);
            float sqrRadius = sumRadius * sumRadius;

            float distSqr = (xd * xd) + (yd * yd);

            if (Math.Round(distSqr) < Math.Round(sqrRadius))
            {
                vector = new Vector2D(xd, yd);
            }

            return vector;
        }

        private void frmTable_MouseMove(object sender, MouseEventArgs e)
        {
            if (poolState == PoolState.AwaitingShot)
            {
                if (e.X >= thermometerRectangle.X && e.X <= thermometerRectangle.X + thermometerRectangle.Width && e.Y >= thermometerRectangle.Y - 3 && e.Y <= thermometerRectangle.Y + thermometerRectangle.Height + 3)
                {
                    this.Cursor = Cursors.Hand;
                }
                else
                {
                    bool overBall = false;
                    int ballX = (int)(targetRectangle.X + targetRectangle.Width / 2.0);
                    int ballY = (int)(targetRectangle.Y + targetRectangle.Height / 2.0);
                    int ballRadius = targetRectangle.Width / 2;

                    float xd = (float)(e.X - ballX);
                    float yd = (float)(e.Y - ballY);

                    float sumRadius = (float)(ballRadius);
                    float sqrRadius = ballRadius * ballRadius;

                    float distSqr = (xd * xd) + (yd * yd);

                    if (Math.Round(distSqr) < Math.Round(sqrRadius))
                    {
                        overBall = true;
                    }

                    if (overBall)
                    {
                        this.Cursor = Cursors.Hand;
                    }
                    else
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void frmTable_MouseDown(object sender, MouseEventArgs e)
        {
            TrySetTargetOrStrenght(e.X, e.Y);
        }

        private void TrySetTargetOrStrenght(int x, int y)
        {
            if (!SetStrength(x, y))
            {
                targetVector = SetTarget(x, y);
                lblTarget.Location = new Point((int)(targetRectangle.X + targetRectangle.Width / 2 + targetVector.X - 3), (int)(targetRectangle.Y + targetRectangle.Height / 2 + targetVector.Y - 3));
            }
        }

        private void lblStrenght_MouseDown(object sender, MouseEventArgs e)
        {
            SetStrength(lblStrenght.Left + e.X, lblStrenght.Top + e.Y);
        }

        private void timerComputer_Tick(object sender, EventArgs e)
        {
            GenerateComputerShot();
        }

        private void GenerateComputerShot()
        {
            timerComputer.Enabled = false;

            picBallOn.Visible = true;
            SetBallOnImage();

            List<Ball> auxBalls = new List<Ball>();

            auxBalls.Clear();
            foreach (Ball b in _ballManager.Balls)
            {
                Ball auxBall = new Ball(b.Id, this, (int)b.Position.X, (int)b.Position.Y, b.Image, b.Points);
                auxBall.IsBallInPocket = b.IsBallInPocket;
                auxBalls.Add(auxBall);
            }

            int lastPlayerScore   = _playerManager.CurrentPlayer.Points;
            int lastOpponentScore = _playerManager.OtherPlayer.Points;

            int newPlayerScore = -1;
            int newOpponentScore = 1000;

            int attemptsToWin = 0;
            int attemptsNotToLose = 0;
            int attemptsOfDespair = 0;
            while (true)
            {
                if (attemptsToWin < MAX_COMPUTER_ATTEMPTS)
                {
                    attemptsToWin++;
                }
                else if (attemptsNotToLose < MAX_COMPUTER_ATTEMPTS)
                {
                    attemptsNotToLose++;
                }
                else
                {
                    attemptsOfDespair++;                    
                }

                _playerManager.CurrentPlayer.Points = lastPlayerScore;
                _playerManager.OtherPlayer.Points = lastOpponentScore;

                bool despair = (attemptsOfDespair >= MAX_COMPUTER_ATTEMPTS);
                GenerateRandomTestComputerShot(despair);

                newPlayerScore = _playerManager.CurrentPlayer.Points;
                newOpponentScore = _playerManager.OtherPlayer.Points;

                int i = 0;
                foreach (Ball b in _ballManager.Balls)
                {
                    Ball auxB = auxBalls[i];
                    b.Position.X = auxB.Position.X;
                    b.Position.Y = auxB.Position.Y;
                    b.IsBallInPocket = auxB.IsBallInPocket;
                    i++;
                }

                if (newPlayerScore > lastPlayerScore ||
                    newOpponentScore == lastOpponentScore && (attemptsToWin >= MAX_COMPUTER_ATTEMPTS) ||
                    attemptsOfDespair > 5)
                {
                    _playerManager.CurrentPlayer.Points = lastPlayerScore;
                    _playerManager.OtherPlayer.Points = lastOpponentScore;

                    GenerateLastGoodComputerShot();
                    break;
                }
            }
        }

        private void GenerateRandomTestComputerShot(bool despair)
        {
            _playerManager.CurrentPlayer.BallOn = GetNextBallOn(_playerManager.CurrentPlayer.JustSwapped, _playerManager.CurrentPlayer.BallOn);
            Ball ghostBall = null;
            Random rnd = new Random(DateTime.Now.Second);

            List<Ball> ballOnList = new List<Ball>();

            if (_playerManager.CurrentPlayer.BallOn == null)
            {
                ballOnList = _ballManager.GetValidRedBalls();
            }
            else if (_playerManager.CurrentPlayer.BallOn.Points == 1)
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
                        if (b.Points > 1 && !b.IsBallInPocket)
                            ballOnList.Add(b);
                    }
                }
                else
                {
                    ballOnList.Add(_playerManager.CurrentPlayer.BallOn);
                }
            }

            ghostBall = GetRandomGhostBall(ballOnList, despair);

            if (ghostBall == null)
                ghostBall = GetNextBallOn(_playerManager.CurrentPlayer.JustSwapped, _playerManager.CurrentPlayer.BallOn);

            rnd = new Random(DateTime.Now.Second);
            int strength = rnd.Next(15);

            int strenghtBase = 45;

            _playerManager.CurrentPlayer.Strength = strenghtBase + strength;

            if (ghostBall != null)
            {
                _playerManager.CurrentPlayer.TestPosition = new Vector2D((int)ghostBall.X, (int)ghostBall.Y);
                _playerManager.CurrentPlayer.TestStrength = _playerManager.CurrentPlayer.Strength;
                HitBall((int)ghostBall.X, (int)ghostBall.Y, true);
            }
        }

        private void GenerateLastGoodComputerShot()
        {
            int x = (int)_playerManager.CurrentPlayer.TestPosition.X;
            int y = (int)_playerManager.CurrentPlayer.TestPosition.Y;
            _playerManager.CurrentPlayer.Strength = _playerManager.CurrentPlayer.TestStrength;

            tableGraphics.DrawEllipse(new Pen(Brushes.Black), new Rectangle((int)(x - Ball.Radius) + 1, (int)(y - Ball.Radius) + 1, (int)Ball.Radius * 2 - 1, (int)Ball.Radius * 2 - 1));
            Point p1 = new Point((int)(x + 1), (int)(y - Ball.Radius * 2.0) + 1);
            Point p2 = new Point((int)(x + 1), (int)(y + Ball.Radius * 2.0) + 1);
            Point p3 = new Point((int)(x - Ball.Radius * 2.0 + 1), (int)(y) - 1);
            Point p4 = new Point((int)(x + Ball.Radius * 2.0 + 1), (int)(y) - 1);
            tableGraphics.DrawLine(new Pen(Brushes.Black), p1, p2);
            tableGraphics.DrawLine(new Pen(Brushes.White), p3, p4);

            tableGraphics.DrawEllipse(new Pen(Brushes.White), new Rectangle((int)(x - Ball.Radius), (int)(y - Ball.Radius), (int)Ball.Radius * 2 - 1, (int)Ball.Radius * 2 - 1));
            p1 = new Point((int)(x), (int)(y - Ball.Radius * 2.0));
            p2 = new Point((int)(x), (int)(y + Ball.Radius * 2.0));
            p3 = new Point((int)(x - Ball.Radius * 2.0), (int)(y) - 1);
            p3 = new Point((int)(x + Ball.Radius * 2.0), (int)(y) - 1);
            tableGraphics.DrawLine(new Pen(Brushes.White), p1, p2);
            tableGraphics.DrawLine(new Pen(Brushes.White), p3, p4);

            HitBall(x, y, false);
        }

        private Ball GetNextBallOn(bool swappedPlayers, Ball lastBallOn)
        {
            Ball nextBallOn = null;
            if (playerState == PlayerState.Aiming)
            {
                if (swappedPlayers)
                {
                    if (_playerManager.CurrentPlayer.BallOn == null)
                    {
                        nextBallOn = _ballManager.GetRandomRedBall();
                    }
                    else if (_playerManager.CurrentPlayer.JustSwapped)
                    {
                        nextBallOn = _ballManager.GetRandomRedBall();
                    }
                    else if (_playerManager.CurrentPlayer.BallOn.Points == 1)
                    {
                        nextBallOn = _ballManager.GetRandomRedBall();
                    }

                    if (nextBallOn == null)
                    {
                        nextBallOn = _ballManager.GetMinColouredball();
                    }
                }
                else
                {
                    if (lastBallOn == null)
                    {
                        nextBallOn = _ballManager.GetRandomRedBall();
                    }
                    else if (lastBallOn.Points == 1)
                    {
                        nextBallOn = _ballManager.GetMinColouredball();
                    }
                    else
                    {
                        nextBallOn = _ballManager.GetRandomRedBall();
                        if (nextBallOn == null)
                        {
                            nextBallOn = _ballManager.GetMinColouredball();
                        }
                    }
                }
            }
            else
            {
                nextBallOn = _ballManager.GetMinColouredball();
            }
            return nextBallOn;
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
                    Ball mirroredBall = new Ball("m1", null, (int)(ballOn.X - Ball.Radius), (int)(-1.0 * ballOn.Y), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball("m2", null, (int)(-1.0 * ballOn.X), (int)(ballOn.Y), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball("m3", null, (int)(ballOn.X), (int)(ballOn.Y + (picTable.Height * 2.0)), null, ballOn.Points);
                    tempGhostBalls = GetGhostBalls(mirroredBall, despair);
                    foreach (Ball ghostBall in tempGhostBalls)
                    {
                        ghostBalls.Add(ghostBall);
                    }
                    mirroredBall = new Ball("m4", null, (int)(ballOn.X + (picTable.Width * 2.0)), (int)(ballOn.Y), null, ballOn.Points);
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
            foreach (Pocket pocket in pockets)
            {
                //distances between pocket and ball on center
                double dxPocketBallOn = pocket.HotSpotX - ballOn.X;
                double dyPocketBallOn = pocket.HotSpotY - ballOn.Y;
                double hPocketBallOn = Math.Sqrt(dxPocketBallOn * dxPocketBallOn + dyPocketBallOn * dyPocketBallOn);
                double a = dyPocketBallOn / dxPocketBallOn;

                //distances between ball on center and ghost ball center
                double hBallOnGhost = (Ball.Radius - 1.5) * 2.0;
                double dxBallOnGhost = hBallOnGhost * (dxPocketBallOn / hPocketBallOn);
                double dyBallOnGhost = hBallOnGhost * (dyPocketBallOn / hPocketBallOn);

                //ghost ball coordinates
                double gX = ballOn.X - dxBallOnGhost;
                double gY = ballOn.Y - dyBallOnGhost;
                double dxGhostCue = _ballManager.Balls[0].X - gX;
                double dyGhostCue = _ballManager.Balls[0].Y - gY;
                double hGhostCue = Math.Sqrt(dxGhostCue * dxGhostCue + dyGhostCue * dyGhostCue);

                //distances between ball on center and cue ball center
                double dxBallOnCueBall = ballOn.X - _ballManager.Balls[0].X;
                double dyBallOnCueBall = ballOn.Y - _ballManager.Balls[0].Y;
                double hBallOnCueBall = Math.Sqrt(dxBallOnCueBall * dxBallOnCueBall + dyBallOnCueBall * dyBallOnCueBall);

                //discards difficult ghost balls
                if (despair || (Math.Sign(dxPocketBallOn) == Math.Sign(dxBallOnCueBall) && Math.Sign(dyPocketBallOn) == Math.Sign(dyBallOnCueBall)))
                {
                    Ball ghostBall = new Ball(i.ToString(), null, (int)gX, (int)gY, null, 0);
                    ghostBalls.Add(ghostBall);
                    i++;
                }
            }

            return ghostBalls;
        }

        private int GetRandomStrenght()
        {
            Random rnd = new Random(DateTime.Now.Second);
            return rnd.Next(20) + 30;
        }

        private void ShowWinner()
        {
            ShowPoints();

            if (_playerManager.CurrentPlayer.Points == _playerManager.OtherPlayer.Points)
            {
                lblWin.Text = "Draw!";
            }
            else if (_playerManager.OtherPlayer.Points < _playerManager.CurrentPlayer.Points)
            {
                lblWin.Text = string.Format("{0} Wins!", _playerManager.CurrentPlayer.Name);
            }
            else
            {
                lblWin.Text = string.Format("{0} Wins!", _playerManager.OtherPlayer.Name);
            }

            picTable.Image = Image.FromFile(@"Images\tableBlue.JPG");

            picTable.Update();

            ClearFramesAndSounds();
            _ballManager.Load(this);

            for (int i = 0; i < 20; i++)
            {
                lblWin.Visible = false;
                lblWin.Invalidate();
                lblWin.Refresh();
                Thread.Sleep(100);
                lblWin.Visible = true;
                lblWin.Invalidate();
                lblWin.Refresh();
                Thread.Sleep(100);
            }

            ShowPoints();

            _playerManager.EndMatch();

            lblWin.Visible = false;
            picBallOn.Image = _ballManager.Balls[1].Image;
            SetBallOnImage();
        }
    }
}