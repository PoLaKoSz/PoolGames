using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

    public enum BallValues
    {
        White = 0,
        Red = 1,
        Yellow = 2,
        Green = 3,
        Brown = 4,
        Blue = 5,
        Pink = 6,
        Black = 7
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


        bool showBallOn = true;
        int snapShotCount = 0;
        int maxSnapShot = 0;
        int currentSnapShot = 1;
        int videoRefreshRate = 2;
        const int MAX_COMPUTER_ATTEMPTS = 2;
        Vector2D targetVector = new Vector2D(0, 0);

        Rectangle thermometerRectangle = new Rectangle(32, 249, 152, 10);
        Rectangle targetRectangle = new Rectangle(68, 269, 71, 71);

        PoolState poolState = PoolState.AwaitingShot;
        Single friction = 0.0075F;
        List<Ball> balls = new List<Ball>();
        List<Pocket> pockets = new List<Pocket>();
        List<TableBorder> tableBorders = new List<TableBorder>();
        List<DiagonalBorder> diagonalBorders = new List<DiagonalBorder>();
        Image imgTable;
        Image imgQuestionBall;
        Image imgShadow;
        Graphics tableGraphics;
        private Bitmap bufferedBitmap;
        private Graphics bufferedGraphics;
        ImageAttributes attr = new ImageAttributes();
        List<Ball> pottedBalls = new List<Ball>();
        List<Ball> fallenBalls = new List<Ball>();
        List<Ball> strokenBalls = new List<Ball>();
        int moveCount = 0;
        List<Bitmap> whiteBitmapList = new List<Bitmap>();
        List<Graphics> whiteBitmapGraphicsList = new List<Graphics>();
        PlayerState playerState = PlayerState.None;
        List<BallPosition> ballPositionList = new List<BallPosition>();
        int gifIndex = 1;



        public MainForm()
        {
            _playerManager = new PlayerManager(new Player(1, "Computer", isComputer: true), new Player(2, "Human"));
            _soundManager = new SoundManager();

            InitializeComponent();

            lblPlayer1Name.Text = _playerManager.CurrentPlayer.Name;
            lblPlayer2Name.Text = _playerManager.OtherPlayer.Name;

            imgTable = Image.FromFile(@"Images\tableBlue.jpg");

            ClearFramesAndSounds();

            imgShadow = Image.FromFile(@"Images\ShadowBall.PNG");
            imgQuestionBall = Image.FromFile(@"Images\questionball.PNG");

            LoadBalls();

            pockets.Add(new Pocket(this, 1, 5, 5, 29, 29));
            pockets.Add(new Pocket(this, 2, 288, 0, 301, 25));
            pockets.Add(new Pocket(this, 3, 571, 5, 573, 29));
            pockets.Add(new Pocket(this, 4, 5, 309, 29, 309));
            pockets.Add(new Pocket(this, 5, 288, 314, 301, 313));
            pockets.Add(new Pocket(this, 6, 571, 309, 572, 310));

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

            attr.SetColorKey(Color.White, Color.White);

            lblStrenght.Width = (int)((_playerManager.CurrentPlayer.Strength * (thermometerRectangle.Width - 12) / 100.0));
            _playerManager.CurrentPlayer.BallOn = balls[1];
            SetBallOnImage();

            timerInBox.Enabled = _playerManager.CurrentPlayer.IsComputer;

            UpdatePlayerState(PlayerState.Aiming);
            _playerManager.CurrentPlayer.BallOn = GetRandomRedBall();
            _playerManager.CurrentPlayer.Strength = GetRandomStrenght();
            SetBallOnImage();
            timerComputer.Enabled = true;
        }



        private void ClearFramesAndSounds()
        {
            _soundManager.Empty();
            whiteBitmapList.Clear();
            whiteBitmapGraphicsList.Clear();

            for (int i = 0; i < 300; i++)
            {
                Bitmap bmp = new Bitmap(imgTable);
                whiteBitmapList.Add(bmp);
                whiteBitmapGraphicsList.Add(Graphics.FromImage(bmp));
                _soundManager.Add();
            }
        }

        private void LoadBalls()
        {
            Image imgRedBall = Image.FromFile(@"Images\RedBall.PNG");
            Image imgWhiteBall = Image.FromFile(@"Images\whiteball.PNG");
            Image imgYellowBall = Image.FromFile(@"Images\YellowBall.PNG");
            Image imgGreenBall = Image.FromFile(@"Images\GreenBall.PNG");
            Image imgBrownBall = Image.FromFile(@"Images\BrownBall.PNG");
            Image imgBlackBall = Image.FromFile(@"Images\BlackBall.PNG");
            Image imgPinkBall = Image.FromFile(@"Images\PinkBall.PNG");
            Image imgBlueBall = Image.FromFile(@"Images\BlueBall.PNG");

            balls.Clear();
            Ball ball01 = new Ball("white", this, 497, 140, imgWhiteBall, (int)BallValues.White);

            Ball ball02 = new Ball("red01", this, 121, 152, imgRedBall, (int)BallValues.Red);
            Ball ball03 = new Ball("red02", this, 121, 171, imgRedBall, 1);
            Ball ball04 = new Ball("red03", this, 121, 190, imgRedBall, 1);
            Ball ball05 = new Ball("red04", this, 140, 162, imgRedBall, 1);
            Ball ball06 = new Ball("red05", this, 140, 180, imgRedBall, 1);
            Ball ball07 = new Ball("red06", this, 159, 171, imgRedBall, 1);

            Ball ball08 = new Ball("yellow", this, 469, 115, imgYellowBall, (int)BallValues.Yellow);
            Ball ball10 = new Ball("green",  this, 469, 228, imgGreenBall, (int)BallValues.Green);
            Ball ball09 = new Ball("brown",  this, 469, 171, imgBrownBall, (int)BallValues.Brown);
            Ball ball13 = new Ball("blue",   this, 298, 171, imgBlueBall, (int)BallValues.Blue);
            Ball ball12 = new Ball("pink",   this, 178, 171, imgPinkBall, (int)BallValues.Pink);
            Ball ball11 = new Ball("black",  this,  50, 171, imgBlackBall, (int)BallValues.Black);

            balls.Add(ball01);
            balls.Add(ball02);
            balls.Add(ball03);
            balls.Add(ball04);
            balls.Add(ball05);
            balls.Add(ball06);
            balls.Add(ball07);

            balls.Add(ball08);
            balls.Add(ball09);
            balls.Add(ball10);
            balls.Add(ball11);
            balls.Add(ball12);
            balls.Add(ball13);
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
            foreach (Ball ball in balls)
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
                    foreach (Ball ball in balls)
                    {
                        foreach (Pocket pocket in pockets)
                        {
                            bool inPocket = pocket.IsBallInPocket(ball);
                        }
                    }

                    someCollision = false;
                    foreach (Ball ballA in balls)
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

                        foreach (Ball ballB in balls)
                        {
                            if (ballA.Id.CompareTo(ballB.Id) != 0)
                            {
                                if (ballA.Colliding(ballB) && !ballA.IsBallInPocket && !ballB.IsBallInPocket)
                                {
                                    if (ballA.Points == 0)
                                    {
                                        strokenBalls.Add(ballB);
                                    }
                                    else if (ballB.Points == 0)
                                    {
                                        strokenBalls.Add(ballA);
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

                    foreach (Ball ball in balls)
                    {
                        ball.Position.X += ball.TranslateVelocity.X + ball.VSpinVelocity.X;
                        ball.Position.Y += ball.TranslateVelocity.Y + ball.VSpinVelocity.Y;
                    }
                }

                MoveBall(false);
                conflicted = false;
            }

            double totalVelocity = 0;
            foreach (Ball ball in balls)
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
            maxSnapShot = snapShotCount;

            if (!test)
            {
                DrawSnapShots();
                PlayLastShot();
            }

            ProcessFallenBalls(test);

            if (playerState != PlayerState.GameOver)
            {
                ballPositionList.Clear();
                MoveBall(true);

                if (!test)
                {
                    DrawSnapShots();
                    PlayLastShot();
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

                ballPositionList.Clear();

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

        private void PlayLastShot()
        {
            currentSnapShot = 1;
            if (ballPositionList.Count > 0)
            {
                maxSnapShot = ballPositionList[ballPositionList.Count - 1].SnapShot + 1;
            }

            gifIndex = 1;
            while (currentSnapShot <= maxSnapShot)
            {
                Thread.Sleep(videoRefreshRate * 10);
                PlaySnapShot();
            }
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

        private void ClearSequenceBackGround()
        {
            for (int i = 0; i < snapShotCount; i++)
            {
                whiteBitmapList[i].Dispose();
                whiteBitmapGraphicsList[i].Dispose();
            }

            imgTable = Image.FromFile(@"Images\tableBlue.JPG");
            for (int i = 0; i < snapShotCount; i++)
            {
                Bitmap bmp = new Bitmap(imgTable);
                whiteBitmapList[i] = bmp;
                whiteBitmapGraphicsList[i] = Graphics.FromImage(bmp);
            }
            snapShotCount = 0;
        }

        private void ProcessFallenBalls(bool test)
        {
            _playerManager.CurrentPlayer.FoulList.Clear();

            int redCount = 0;
            int fallenRedCount = 0;
            int wonPoints = 0;
            int lostPoints = 0;
            bool someInTable = false;

            foreach (Ball ball in balls)
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

            foreach (Ball ball in balls)
            {
                if (ball.Points == 1 && ball.IsBallInPocket)
                {
                    fallenRedCount++;
                }
            }

            foreach (Ball ball in pottedBalls)
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
                            Ball candidateBall = GetCandidateBall(ball, points);
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
                _playerManager.CurrentPlayer.BallOn = balls[1];

            int strokenBallsCount = 0;
            foreach (Ball ball in strokenBalls)
            {
                //causing the cue ball to first hit a ball other than the ball on
                if (strokenBallsCount == 0 && ball.Points != _playerManager.CurrentPlayer.BallOn.Points)
                    _playerManager.CurrentPlayer.FoulList.Add((_playerManager.CurrentPlayer.BallOn.Points < 4 ? 4 : _playerManager.CurrentPlayer.BallOn.Points));

                strokenBallsCount++;
            }

            //Foul: causing the cue ball to miss all object balls
            if (strokenBallsCount == 0)
                _playerManager.CurrentPlayer.FoulList.Add(4);

            foreach (Ball ball in pottedBalls)
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
                foreach (Ball ball in pottedBalls)
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

                int fallenBallsCount = fallenBalls.Count;
                for (int i = fallenBallsCount - 1; i >= 0; i--)
                {
                    if (!fallenBalls[i].IsBallInPocket)
                    {
                        fallenBalls.RemoveAt(i);
                    }
                }

                ballPositionList.Clear();

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

            strokenBalls.Clear();
            pottedBalls.Clear();
            _soundManager.Empty();

            if (!_playerManager.CurrentPlayer.IsComputer)
                timerInBox.Enabled = true;
        }

        private Ball GetMinColouredball()
        {
            Ball minColouredball = null;
            int minPoints = 8;
            foreach (Ball ball in balls)
            {
                if (ball.Points > 1 && ball.Points < minPoints && !ball.IsBallInPocket)
                {
                    minColouredball = ball;
                    minPoints = minColouredball.Points;
                }
            }
            return minColouredball;
        }

        private Ball GetCandidateBall(Ball ball, int points)
        {
            Ball candidateBall = null;
            Ball fallenBall = ball;
            while (candidateBall == null)
            {
                foreach (Ball b in balls)
                {
                    if (b.Points == points)
                    {
                        candidateBall = b;
                    }
                }
                if (candidateBall != null)
                {
                    foreach (Ball collisionBall in balls)
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

        private void MoveBall(bool forcePaint)
        {
            moveCount++;

            if (moveCount < videoRefreshRate && !forcePaint)
            {
                return;
            }
            else
            {
                moveCount = 0;
            }

            bool someMoved = false;

            foreach (Ball ball in balls)
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
                foreach (Ball ball in balls)
                {
                    int lastX = (int)ball.LastX;
                    int X = (int)ball.X;
                    int lastY = (int)ball.LastY;
                    int Y = (int)ball.Y;

                    if (!ball.IsBallInPocket)
                    {
                        ballPositionList.Add(new BallPosition(snapShotCount, index, X, Y));

                        ball.LastX = ball.X;
                        ball.LastY = ball.Y;
                    }
                    index++;
                }

                snapShotCount++;
            }
        }

        private void DrawSnapShots()
        {
            ClearSequenceBackGround();

            int snapShot = -1;

            Graphics whiteBitmapGraphics = null;

            //For each ball, draws an image of that ball over the pool background image
            foreach (BallPosition ballPosition in ballPositionList)
            {
                if (ballPosition.SnapShot != snapShot)
                {
                    snapShot = ballPosition.SnapShot;
                    whiteBitmapGraphics = whiteBitmapGraphicsList[snapShot];
                }

                //draws an image of a ball over the pool background image
                whiteBitmapGraphics.DrawImage(balls[ballPosition.BallIndex].Image, new Rectangle((int)(ballPosition.X - Ball.Radius), (int)(ballPosition.Y - Ball.Radius), (int)Ball.Radius * 2, (int)Ball.Radius * 2), 0, 0, (int)Ball.Radius * 2, (int)Ball.Radius * 2, GraphicsUnit.Pixel, attr);
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
                        bufferedBitmap = new Bitmap(picTable.Width, picTable.Height);
                        bufferedGraphics = Graphics.FromImage(bufferedBitmap);
                        MoveBall(true);
                        DrawSnapShots();
                        PlaySnapShot();
                        ballPositionList.Clear();
                        currentSnapShot = 1;
                        snapShotCount = 0;
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
            ClearSequenceBackGround();
            ballPositionList.Clear();

            poolState = PoolState.Moving;
            picTable.Cursor = Cursors.WaitCursor;

            //20 is the maximum velocity
            double v = 20 * (_playerManager.CurrentPlayer.Strength / 100.0);

            //Calculates the cue angle, and the translate velocity (normal velocity)
            double dx = x - balls[0].X;
            double dy = y - balls[0].Y;
            double h = (double)(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
            double sin = dy / h;
            double cos = dx / h;

            balls[0].IsBallInPocket = false;
            balls[0].TranslateVelocity.X = v * cos;
            balls[0].TranslateVelocity.Y = v * sin;
            Vector2D normalVelocity = balls[0].TranslateVelocity.Normalize();

            //Calculates the top spin/back spin velocity, in the same direction as the normal velocity, but in opposite angle
            double topBottomVelocityRatio = balls[0].TranslateVelocity.Lenght() * (targetVector.Y / 100.0);
            balls[0].VSpinVelocity = new Vector2D(-1.0d * topBottomVelocityRatio * normalVelocity.X, -1.0d * topBottomVelocityRatio * normalVelocity.Y);
            
            _soundManager.Add(snapShotCount, new ShotSound(balls[0]));

            //Calculates the ball positions as long as there are moving balls
            while (poolState == PoolState.Moving)
                MoveBalls(test);

            AfterBallsGetStill(test);

            _playerManager.CurrentPlayer.ShotCount++;
        }

        private void frmTable_Deactivate(object sender, EventArgs e)
        {
            foreach (Ball ball in balls)
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
            _soundManager.Add(snapShotCount, new HitSound(volume, ball));
        }

        public void BallDropped(Ball ball)
        {
            _soundManager.Add(snapShotCount, new FallSound(ball));

            fallenBalls.Add(ball);
            pottedBalls.Add(ball);

            ball.IsBallInPocket = true;
        }

        public void WallCollision(Ball ball)
        {
            _soundManager.Add(snapShotCount, new BankSound(ball));
        }

        private void PlaySnapShot()
        {
            //Plays an individual frame, by replacing the image of the picturebox with
            //the stored image of a frame
            picTable.Image = whiteBitmapList[currentSnapShot - 1]; ;
            picTable.Refresh();

            _soundManager.Play(currentSnapShot - 1);

            currentSnapShot++;
        }

        private void picTable_MouseUp(object sender, MouseEventArgs e)
        {
            if (playerState == PlayerState.Calling || playerState == PlayerState.Aiming)
            {
                if (e.Button == MouseButtons.Left && !_playerManager.CurrentPlayer.IsComputer)
                {
                    if (playerState == PlayerState.Calling)
                    {
                        Ball bOn = GetBallOn(e.X, e.Y);
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
                bOn = GetBallOn(x, y);
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

        private Ball GetBallOn(int x, int y)
        {
            Ball bOn = null;
            foreach (Ball ball in balls)
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
            foreach (Ball b in balls)
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
                foreach (Ball b in balls)
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
                ballOnList = GetValidRedBalls();
            }
            else if (_playerManager.CurrentPlayer.BallOn.Points == 1)
            {
                ballOnList = GetValidRedBalls();
            }
            else
            {
                Ball redBall = GetRandomRedBall();
                if (redBall != null)
                {
                    foreach (Ball b in balls)
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
                        nextBallOn = GetRandomRedBall();
                    }
                    else if (_playerManager.CurrentPlayer.JustSwapped)
                    {
                        nextBallOn = GetRandomRedBall();
                    }
                    else if (_playerManager.CurrentPlayer.BallOn.Points == 1)
                    {
                        nextBallOn = GetRandomRedBall();
                    }

                    if (nextBallOn == null)
                    {
                        nextBallOn = GetMinColouredball();
                    }
                }
                else
                {
                    if (lastBallOn == null)
                    {
                        nextBallOn = GetRandomRedBall();
                    }
                    else if (lastBallOn.Points == 1)
                    {
                        nextBallOn = GetMinColouredball();
                    }
                    else
                    {
                        nextBallOn = GetRandomRedBall();
                        if (nextBallOn == null)
                        {
                            nextBallOn = GetMinColouredball();
                        }
                    }
                }
            }
            else
            {
                nextBallOn = GetMinColouredball();
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
                double dxGhostCue = balls[0].X - gX;
                double dyGhostCue = balls[0].Y - gY;
                double hGhostCue = Math.Sqrt(dxGhostCue * dxGhostCue + dyGhostCue * dyGhostCue);

                //distances between ball on center and cue ball center
                double dxBallOnCueBall = ballOn.X - balls[0].X;
                double dyBallOnCueBall = ballOn.Y - balls[0].Y;
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

        private Ball GetRandomRedBall()
        {
            Ball redBallOn = null;

            List<int> validRedBalls = new List<int>();
            int i = 0;
            foreach (Ball ball in balls)
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

                redBallOn = balls[validRedBalls[index]];
            }
            return redBallOn;
        }

        private List<Ball> GetValidRedBalls()
        {
            List<Ball> validRedBalls = new List<Ball>();

            foreach (Ball ball in balls)
            {
                if (ball.Points == 1 && !ball.IsBallInPocket)
                {
                    validRedBalls.Add(ball);
                }
            }
            return validRedBalls;
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
            LoadBalls();

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
            picBallOn.Image = balls[1].Image;
            SetBallOnImage();
        }
    }
}