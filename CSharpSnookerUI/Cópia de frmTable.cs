using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SkypeSnookerUI
{
    public enum PoolState
    {
        AwaitingShot,
        Moving
    }

    public partial class frmTable : Form, IBallObserver, IHoleObserver
    {
        int snapShotCount = 0;
        PoolState poolState = PoolState.AwaitingShot;
        //Single peakVelocity = 10;
        Single peakVelocity = 3;
        Single friction = 0.01F;
        //Single friction = 1.00F;
        Single absorption = 0.80F;
        //Single absorption = 1.00F;
        List<Ball> balls = new List<Ball>();
        List<Hole> holes = new List<Hole>();
        List<TableBorder> tableBorders = new List<TableBorder>();
        List<DiagonalBorder> diagonalBorders = new List<DiagonalBorder>();
        Image imgTable;
        Graphics tableGraphics;
        private Bitmap bufferedBitmap;
        private Graphics bufferedGraphics;
        ImageAttributes attr = new ImageAttributes();
        System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
        int progressDirection = 1;
        int points1 = 0;
        int points2 = 0;
        int fallenBalls = 0;
        int moveCount = 0;

        public frmTable()
        {
            string hitSound = @"single.wav";
            string pocketSound = @"pool.wav";
            InitializeComponent();
            imgTable = Image.FromFile("table.PNG");
            Image imgRedBall = Image.FromFile("RedBall.PNG");
            Image imgWhiteBall = Image.FromFile("whiteball.PNG");
            Image imgYellowBall = Image.FromFile("YellowBall.PNG");
            Image imgGreenBall = Image.FromFile("GreenBall.PNG");
            Image imgBrownBall = Image.FromFile("BrownBall.PNG");
            Image imgBlackBall = Image.FromFile("BlackBall.PNG");
            Image imgPinkBall = Image.FromFile("PinkBall.PNG");
            Image imgBlueBall = Image.FromFile("BlueBall.PNG");

            //Ball ball01 = new Ball("01", this, 313, 187, soundLocation, imgWhiteBall);
            //Ball ball01 = new Ball("01", this, 267, 79, soundLocation, imgWhiteBall);

            Ball ball01 = new Ball("01", this, 497, 150, hitSound, pocketSound, imgWhiteBall);
            //Ball ball02 = new Ball("02", this, 130, 140, hitSound, pocketSound, imgRedBall);
            //Ball ball03 = new Ball("03", this, 230, 140, hitSound, pocketSound, imgRedBall);
            //Ball ball04 = new Ball("04", this, 330, 140, hitSound, pocketSound, imgRedBall);
            //Ball ball05 = new Ball("05", this, 430, 140, hitSound, pocketSound, imgRedBall);
            //Ball ball06 = new Ball("06", this, 530, 140, hitSound, pocketSound, imgRedBall);
            //Ball ball07 = new Ball("07", this, 630, 140, hitSound, pocketSound, imgRedBall);

            Ball ball02 = new Ball("02", this, 121, 135, hitSound, pocketSound, imgRedBall);
            Ball ball03 = new Ball("03", this, 121, 173, hitSound, pocketSound, imgRedBall);
            Ball ball04 = new Ball("04", this, 121, 211, hitSound, pocketSound, imgRedBall);
            Ball ball05 = new Ball("05", this, 140, 154, hitSound, pocketSound, imgRedBall);
            Ball ball06 = new Ball("06", this, 140, 192, hitSound, pocketSound, imgRedBall);
            Ball ball07 = new Ball("07", this, 159, 173, hitSound, pocketSound, imgRedBall);

            Ball ball08 = new Ball("08", this, 469, 122, hitSound, pocketSound, imgYellowBall);
            Ball ball09 = new Ball("09", this, 469, 178, hitSound, pocketSound, imgBrownBall);
            Ball ball10 = new Ball("10", this, 469, 235, hitSound, pocketSound, imgGreenBall);

            Ball ball11 = new Ball("11", this, 50, 173, hitSound, pocketSound, imgBlackBall);
            Ball ball12 = new Ball("12", this, 240, 173, hitSound, pocketSound, imgPinkBall);
            Ball ball13 = new Ball("13", this, 270, 173, hitSound, pocketSound, imgBlueBall);

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

            //balls.Add(ball14);
            //balls.Add(ball15);

            holes.Add(new Hole(this, 1, 0, 0, new Vector2D(0, 0)));
            holes.Add(new Hole(this, 2, 288, 0, new Vector2D(0, 1)));
            holes.Add(new Hole(this, 3, 576, 0, new Vector2D(0, 0)));
            holes.Add(new Hole(this, 4, 0, 314, new Vector2D(0, 0)));
            holes.Add(new Hole(this, 5, 288, 314, new Vector2D(0, 0)));
            holes.Add(new Hole(this, 6, 576, 314, new Vector2D(0, 0)));

            //diagonalBorders.Add(new DiagonalBorder(100, 200, 300, Side.Southeast));

            //diagonalBorders.Add(new DiagonalBorder(299, 311, 300, Side.Southeast));
            //diagonalBorders.Add(new DiagonalBorder(301, 52, 300, Side.Northeast));
            //diagonalBorders.Add(new DiagonalBorder(153, 218, 300, Side.Northwest));
            //diagonalBorders.Add(new DiagonalBorder(157, 141, 300, Side.Southwest));

            //diagonalBorders.Add(new DiagonalBorder(254, 92, 160, Side.Southwest));
            //diagonalBorders.Add(new DiagonalBorder(280, 66, 160, Side.Northeast));
            //diagonalBorders.Add(new DiagonalBorder(454, 492, 40, Side.Southwest));
            //diagonalBorders.Add(new DiagonalBorder(480, 466, 40, Side.Northeast));

            diagonalBorders.Add(new DiagonalBorder(552, 314, 50, Side.Southwest));
            diagonalBorders.Add(new DiagonalBorder(578, 291, 50, Side.Northeast));
            diagonalBorders.Add(new DiagonalBorder(1, 27, 28, Side.Southwest));
            diagonalBorders.Add(new DiagonalBorder(27, 1, 28, Side.Northeast));
            diagonalBorders.Add(new DiagonalBorder(553, 28, 28, Side.Northwest));
            diagonalBorders.Add(new DiagonalBorder(579, 54, 28, Side.Southeast));
            diagonalBorders.Add(new DiagonalBorder(1, 319, 28, Side.Northwest));
            diagonalBorders.Add(new DiagonalBorder(25, 344, 28, Side.Southeast));

            tableBorders.Add(new TableBorder(0, 45, 20, 250, ForcedDirection.None));
            tableBorders.Add(new TableBorder(586, 45, 20, 250, ForcedDirection.None));
            tableBorders.Add(new TableBorder(45, 0, 242, 20, ForcedDirection.None));
            tableBorders.Add(new TableBorder(45, 322, 242, 20, ForcedDirection.None));
            tableBorders.Add(new TableBorder(324, 0, 238, 20, ForcedDirection.None));
            tableBorders.Add(new TableBorder(324, 322, 238, 20, ForcedDirection.None));

            tableBorders.Add(new TableBorder(-20, 0, 20, 344, ForcedDirection.None));
            tableBorders.Add(new TableBorder(606, 0, 20, 344, ForcedDirection.None));
            tableBorders.Add(new TableBorder(0, -20, 606, 20, ForcedDirection.None));
            tableBorders.Add(new TableBorder(0, 344, 606, 20, ForcedDirection.None));

            //tableBorders.Add(new TableBorder(0, 32, 10, 13, ForcedDirection.Up));
            //tableBorders.Add(new TableBorder(32, 0, 13, 10, ForcedDirection.Left));
            //tableBorders.Add(new TableBorder(32, 332, 13, 10, ForcedDirection.Left));
            //tableBorders.Add(new TableBorder(560, 0, 13, 10, ForcedDirection.Right));
            //tableBorders.Add(new TableBorder(560, 332, 13, 10, ForcedDirection.Up));

            //tableBorders.Add(new TableBorder(596, 32, 10, 13, ForcedDirection.Up));
            //tableBorders.Add(new TableBorder(596, 295, 10, 10, ForcedDirection.Down));

            attr.SetColorKey(Color.White, Color.White);
            //MoveBalls();

            //SoundPlayer wavPlayer = new SoundPlayer();
            //wavPlayer.SoundLocation = @"Michael.wav";
            //wavPlayer.LoadCompleted += new AsyncCompletedEventHandler(wavPlayer_LoadCompleted);
            //wavPlayer.LoadAsync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            while (poolState == PoolState.Moving)
                MoveBalls();

            if (poolState == PoolState.AwaitingShot)
            {
                int nextValue = progressBar1.Value + 1 * progressDirection;
                if (nextValue > progressBar1.Maximum || nextValue < 0)
                {
                    progressDirection *= -1;
                    nextValue = progressBar1.Value + 1 * progressDirection;
                }
                progressBar1.Value = nextValue;
            }
            
            timer1.Enabled = true;
        }

        private void MoveBalls()
        {
            foreach (Ball ball in balls)
            {
                if (Math.Abs(ball.X) < 5 && Math.Abs(ball.Y) < 5 && Math.Abs(ball.XVelocity) < 10 && Math.Abs(ball.YVelocity) < 10)
                {
                    ball.X =
                    ball.Y = 0;

                    ball.XVelocity =
                    ball.YVelocity = 0;
                }
            }

            bool conflicted = true;
            int totalPoints1 = 0;
            int totalPoints2 = 0;

            while (conflicted)
            {
                conflicted = false;
                //foreach (Ball ball in balls)
                //{
                //    //int xOffset = (int)(ball.X + Ball.Radius + ball.XVelocity) - pnlTable.Width;
                //    int xOffset = (int)(ball.X + Ball.Radius) - pnlTable.Width;
                //    if (xOffset > 0)
                //    {
                //        ball.X = ball.X - xOffset * 2;
                //        ball.XVelocity *= -1 * absorption;
                //        conflicted = true;
                //    }
                //    //xOffset = (int)(ball.X + ball.XVelocity);
                //    xOffset = (int)(ball.X - Ball.Radius);
                //    if (xOffset < 0)
                //    {
                //        ball.X = ball.X - xOffset * 2;
                //        ball.XVelocity *= -1 * absorption;
                //        conflicted = true;
                //    }
                //    //int yOffset = (int)(ball.Y + Ball.Radius * 2 + ball.YVelocity - pnlTable.Height);
                //    int yOffset = (int)(ball.Y + Ball.Radius) - pnlTable.Height;
                //    if (yOffset > 0)
                //    {
                //        ball.Y = ball.Y - yOffset * 2;
                //        ball.YVelocity *= -1 * absorption;
                //        conflicted = true;
                //    }
                //    //yOffset = (int)(ball.Y + Ball.Radius + ball.YVelocity);
                //    yOffset = (int)(ball.Y - Ball.Radius);
                //    if (yOffset < 0)
                //    {
                //        ball.Y = ball.Y - yOffset * 2;
                //        ball.YVelocity *= -1 * absorption;
                //        conflicted = true;
                //    }
                //}

                //foreach (Ball ballA in balls)
                //{
                bool someCollision = true;
                while (someCollision)
                {
                    someCollision = false;
                    foreach (Ball ballA in balls)
                    {
                        foreach (DiagonalBorder diagonalBorder in diagonalBorders)
                        {
                            if (diagonalBorder.Colliding(ballA))
                            {
                                //listBox1.Items.Add(diagonalBorder.ToString());
                                //listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                //tableGraphics.DrawLine(new Pen(Brushes.White), diagonalBorder.X1, diagonalBorder.Y1, diagonalBorder.X2, diagonalBorder.Y2);
                                diagonalBorder.ResolveCollision(ballA);
                            }
                        }

                        RectangleCollision borderCollision = RectangleCollision.None;
                        foreach (TableBorder tableBorder in tableBorders)
                        {
                            borderCollision = tableBorder.Colliding(ballA);

                            if (borderCollision != RectangleCollision.None)
                            {
                                //listBox1.Items.Add(tableBorder.ToString());
                                //listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                someCollision = true;
                                tableBorder.ResolveCollision(ballA, borderCollision);
                            }
                        }

                        foreach (Ball ballB in balls)
                        {
                            if (ballA.Id.CompareTo(ballB.Id) != 0)
                            {
                                if (ballA.Colliding(ballB))
                                {
                                    ballA.PlaySound();
                                    while (ballA.Colliding(ballB))
                                    {
                                        someCollision = true;
                                        ballA.ResolveCollision(ballB);

                                        ballA.X += (int)ballA.Velocity.X;
                                        ballA.Y += (int)ballA.Velocity.Y;

                                        ballB.X += (int)ballB.Velocity.X;
                                        ballB.Y += (int)ballB.Velocity.Y;
                                    }
                                }
                            }
                        }

                        double absXVelocity = Math.Abs(ballA.XVelocity);
                        double absYVelocity = Math.Abs(ballA.YVelocity);
                        double signalXVelocity = ballA.XVelocity >= 0 ? 1 : -1;
                        double signalYVelocity = ballA.YVelocity >= 0 ? 1 : -1;

                        absXVelocity = absXVelocity * (1 - friction) - 0.01;
                        absYVelocity = absYVelocity * (1 - friction) - 0.01;

                        if (absXVelocity < 0)
                            absXVelocity = 0;

                        if (absYVelocity < 0)
                            absYVelocity = 0;

                        ballA.XVelocity = absXVelocity * signalXVelocity;
                        ballA.YVelocity = absYVelocity * signalYVelocity;

                        MoveBall((int)ballA.X, (int)ballA.Y, false);
                   
                    }                    
                }
                conflicted = false;
            }

            double totalVelocity = 0;
            foreach (Ball ball in balls)
            {
                ball.Position.X += ball.XVelocity;
                ball.Position.Y += ball.YVelocity;
                totalVelocity += ball.XVelocity;
                totalVelocity += ball.YVelocity;
            }

            if (poolState == PoolState.Moving && totalVelocity == 0)
            {
                foreach (Ball ball in balls)
                {
                    MoveBall((int)ball.X, (int)ball.Y, true);
                }

                listBox1.Items.Add(snapShotCount.ToString());
                snapShotCount = 0;
                
                poolState = PoolState.AwaitingShot;
                pnlTable.Cursor = Cursors.SizeAll;
            }

            foreach (Ball ball in balls)
            {
                if (ball.IsBallInHole)
                {
                    //MoveBalls();
                }
                foreach (Hole hole in holes)
                {
                    if (hole.IsBallInHole(ball) && (ball.Velocity.X != 0 || ball.Velocity.Y != 0))
                    {
                        //MoveBalls();
                        //MessageBox.Show(string.Format("Ball {0} in hole {1}", ball.Id, hole.Id));
                    }
                }
            }
        }

        //private void MoveBalls_bkpXXXXXXXXXXXXXXXXXX()
        //{
        //    foreach (Ball ball in balls)
        //    {
        //        if (Math.Abs(ball.X) < 5 && Math.Abs(ball.Y) < 5 && Math.Abs(ball.XVelocity) < 10 && Math.Abs(ball.YVelocity) < 10)
        //        {
        //            //MessageBox.Show("caçapa 1");
        //            ball.X =
        //            ball.Y = 0;

        //            ball.XVelocity =
        //            ball.YVelocity = 0;
        //        }
        //    }

        //    bool conflicted = true;
        //    int totalPoints1 = 0;
        //    int totalPoints2 = 0;

        //    foreach (Ball ball in balls)
        //    {
        //        //ClearBall((int)ball.X, (int)ball.Y);
        //        //totalPoints1 += ball.X + ball.Y;
        //    }

        //    while (conflicted)
        //    {
        //        conflicted = false;
        //        foreach (Ball ball in balls)
        //        {
        //            //int xOffset = (int)(ball.X + Ball.Radius + ball.XVelocity) - pnlTable.Width;
        //            int xOffset = (int)(ball.X + Ball.Radius) - pnlTable.Width;
        //            if (xOffset > 0)
        //            {
        //                ball.X = ball.X - xOffset * 2;
        //                ball.XVelocity *= -1 * absorption;
        //                conflicted = true;
        //            }
        //            //xOffset = (int)(ball.X + ball.XVelocity);
        //            xOffset = (int)(ball.X - Ball.Radius);
        //            if (xOffset < 0)
        //            {
        //                ball.X = ball.X - xOffset * 2;
        //                ball.XVelocity *= -1 * absorption;
        //                conflicted = true;
        //            }
        //            //int yOffset = (int)(ball.Y + Ball.Radius * 2 + ball.YVelocity - pnlTable.Height);
        //            int yOffset = (int)(ball.Y + Ball.Radius) - pnlTable.Height;
        //            if (yOffset > 0)
        //            {
        //                ball.Y = ball.Y - yOffset * 2;
        //                ball.YVelocity *= -1 * absorption;
        //                conflicted = true;
        //            }
        //            //yOffset = (int)(ball.Y + Ball.Radius + ball.YVelocity);
        //            yOffset = (int)(ball.Y - Ball.Radius);
        //            if (yOffset < 0)
        //            {
        //                ball.Y = ball.Y - yOffset * 2;
        //                ball.YVelocity *= -1 * absorption;
        //                conflicted = true;
        //            }
        //        }

        //        //foreach (Ball ballA in balls)
        //        //{
        //        Ball ballA = balls[0];
        //        foreach (Ball ballB in balls)
        //        {
        //            if (ballA.Id.CompareTo(ballB.Id) != 0)
        //            {
        //                int dX = (int)(ballA.X + ballA.XVelocity - ballB.X + ballB.XVelocity);
        //                int dY = (int)(ballA.Y + ballA.YVelocity - ballB.Y + ballB.YVelocity);

        //                //double distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));

        //                float sumRadius = (float)(Ball.Radius * 2);
        //                float sqrRadius = sumRadius * sumRadius;

        //                float distSqr = (dX * dX) + (dY * dY);

        //                //double offset = distance - Ball.Radius * 2;
        //                double sin = dY / (Ball.Radius * 2);
        //                double cos = dX / (Ball.Radius * 2);

        //                while (distSqr <= sqrRadius)
        //                {
        //                    ballA.XVelocity = ballA.XVelocity * -1;
        //                    ballA.YVelocity = ballA.YVelocity * -1;
        //                    ballB.YVelocity = ballB.XVelocity * -1;
        //                    ballB.YVelocity = ballB.YVelocity * -1;

        //                    double velocity = Math.Sqrt(Math.Pow(ballA.XVelocity, 2) + Math.Pow(ballA.YVelocity, 2));

        //                    ballA.X += (int)ballA.XVelocity;
        //                    ballA.Y += (int)ballA.YVelocity;

        //                    ballA.XVelocity = (double)(velocity * cos);// *absorption;
        //                    ballA.YVelocity = (double)(velocity * sin);// * absorption;

        //                    ballB.XVelocity = -1 * (double)(velocity * cos) * absorption;
        //                    ballB.YVelocity = -1 * (double)(velocity * sin) * absorption;

        //                    //ballA.X -= (int)((offset / distance) * dX);
        //                    //ballA.Y -= (int)((offset / distance) * dX);

        //                    //ballB.X += (int)((offset / distance) * dX);
        //                    //ballB.Y += (int)((offset / distance) * dX);

        //                    conflicted = true;

        //                    //dX = (int)(ballA.X - ballB.X + ballB.XVelocity);
        //                    //dY = (int)(ballA.Y - ballB.Y + ballB.YVelocity);

        //                    dX = (int)(ballA.X + ballA.XVelocity - ballB.X + ballB.XVelocity);
        //                    dY = (int)(ballA.Y + ballA.YVelocity - ballB.Y + ballB.YVelocity);

        //                    //double distance = Math.Abs(Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2)));

        //                    //double offset = distance - Ball.Radius * 2;

        //                    distSqr = (dX * dX) + (dY * dY);
        //                }
        //            }
        //        }
        //        //}

        //        foreach (Ball ball in balls)
        //        {
        //            //ClearBall((int)ball.X, (int)ball.Y);
        //            ball.X += (int)ball.XVelocity;
        //            ball.Y += (int)ball.YVelocity;

        //            ball.XVelocity *= friction;
        //            ball.YVelocity *= friction;

        //            //ball.Velocity *= friction;

        //            //if ((int)ball.X != (int)ball.LastX || (int)ball.Y != (int)ball.LastY)
        //            //{
        //            MoveBall((int)ball.X, (int)ball.Y, false);
        //            //ClearBall((int)ball.X, (int)ball.Y);
        //            //}
        //            //else
        //            //{
        //            //    ball.XVelocity =
        //            //    ball.YVelocity = 0;
        //            //}
        //        }
        //        conflicted = false;
        //    }

        //    foreach (Ball ball in balls)
        //    {
        //        //double x = ball.X + ball.XVelocity;
        //        //double y = ball.Y + ball.YVelocity;
        //        //ball.SetPosition(x, y);

        //        ball.X += ball.XVelocity;
        //        ball.Y += ball.YVelocity;
        //    }

        //    if (Math.Abs(totalPoints1 - totalPoints2) < 10)
        //    {
        //        pnlTable.Cursor = Cursors.Cross;
        //        //timer1.Enabled = false;
        //    }
        //}


        private void frmTable_KeyDown(object sender, KeyEventArgs e)
        {
            //switch (e.KeyCode)
            //{
            //    case Keys.Up:
            //        target.Y -= (int)target.Velocity;
            //        target.Velocity = target.Velocity * 1.05F;
            //        break;
            //    case Keys.Down:
            //        target.Y += (int)target.Velocity;
            //        target.Velocity = target.Velocity * 1.05F;
            //        break;
            //    case Keys.Left:
            //        target.X -= (int)target.Velocity;
            //        target.Velocity = target.Velocity * 1.05F;
            //        break;
            //    case Keys.Right:
            //        target.X += (int)target.Velocity;
            //        target.Velocity = target.Velocity * 1.05F;
            //        break;
            //}
        }

        #region IBallObserver Members

        public void BallMoveBegin(double x, double y)
        {
            //ClearBall(x, y);
        }

        public void BallMoveEnd(double x, double y)
        {
            //MoveBall(x, y);
        }

        //private void ClearBall(int x, int y)
        //{
        //    if (bufferedGraphics != null)
        //    {
        //        foreach (Ball ball in balls)
        //        {
        //            //bufferedGraphics.FillRectangle(Brushes.Green, new Rectangle((int)(ball.X - Ball.Radius), (int)(ball.Y + Ball.Radius), 32, 32));
        //            //bufferedGraphics.DrawImage(imgTable, new Rectangle((int)(ball.X - Ball.Radius), (int)(ball.Y + Ball.Radius), 32, 32), new Rectangle((int)(ball.X - Ball.Radius), (int)(ball.Y + Ball.Radius), 32, 32), GraphicsUnit.Pixel);
        //        }
        //    }
        //}

        private void MoveBall(int x, int y, bool forcePaint)
        {
            moveCount++;

            if (moveCount < 40)
            {
                if (!forcePaint)
                {
                    return;
                }
            }
            else
            {
                moveCount = 0;
            }

            snapShotCount++;

            foreach (Ball ball in balls)
            {
                int minX = pnlTable.Width;
                int minY = pnlTable.Height;
                int maxX = 0;
                int maxY = 0;

                if (minX > (int)(ball.X - Ball.Radius))
                    minX = (int)(ball.X - Ball.Radius);

                if (minX > (int)(ball.LastX - Ball.Radius))
                    minX = (int)(ball.LastX - Ball.Radius);

                if (maxX < (int)(ball.X + Ball.Radius))
                    maxX = (int)(ball.X + Ball.Radius);

                if (maxX < (int)(ball.LastX + Ball.Radius))
                    maxX = (int)(ball.LastX + Ball.Radius);

                if (minY > (int)(ball.Y - Ball.Radius))
                    minY = (int)(ball.Y - Ball.Radius);

                if (minY > (int)(ball.LastY - Ball.Radius))
                    minY = (int)(ball.LastY - Ball.Radius);

                if (maxY < (int)(ball.Y + Ball.Radius))
                    maxY = (int)(ball.Y + Ball.Radius);

                if (maxY < (int)(ball.LastY + Ball.Radius))
                    maxY = (int)(ball.LastY + Ball.Radius);

                int aux = 0;

                if (minX > maxX)
                {
                    aux = maxX;
                    maxX = minX;
                    minX = aux;
                }

                if (minY > maxY)
                {
                    aux = maxY;
                    maxY = minY;
                    minY = aux;
                }

                int outParse = 0;
                if (Math.Abs(ball.X) < Int32.MaxValue && Math.Abs(ball.Y) < Int32.MaxValue)
                {
                    if ((int)ball.X != (int)ball.LastX || (int)ball.Y != (int)ball.LastY || ball.IsStill || forcePaint)
                    //if (true)
                    {
                        bufferedBitmap = new Bitmap(maxX - minX, maxY - minY);
                        bufferedGraphics = Graphics.FromImage(bufferedBitmap);

                        bufferedGraphics.DrawImage(imgTable, new Rectangle(0, 0, maxX - minX, maxY - minY), new Rectangle(minX, minY, maxX - minX, maxY - minY), GraphicsUnit.Pixel);

                        if (!ball.IsBallInHole)
                            bufferedGraphics.DrawImage(ball.Image, new Rectangle((int)(ball.X - Ball.Radius) - minX, (int)(ball.Y - Ball.Radius) - minY, (int)(Ball.Radius * 2), (int)(Ball.Radius * 2)), 0, 0, (int)(Ball.Radius * 2), (int)(Ball.Radius * 2), GraphicsUnit.Pixel, attr);

                        tableGraphics.DrawImage(bufferedBitmap, (float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));

                        ball.LastX = ball.X;
                        ball.LastY = ball.Y;
                    }
                }
            }
            //tableGraphics.DrawImage(bufferedBitmap, 0, 0, pnlTable.Width, pnlTable.Height);
            //}

            bufferedBitmap.Save(string.Format(@"SnapShots\{0}.jpg", snapShotCount.ToString("000")));
        }

        #endregion

        private void frmTable_Paint(object sender, PaintEventArgs e)
        {
            if (tableGraphics == null)
            {
                tableGraphics = pnlTable.CreateGraphics();
                bufferedBitmap = new Bitmap(pnlTable.Width, pnlTable.Height);
                bufferedGraphics = Graphics.FromImage(bufferedBitmap);
                foreach (Ball ball in balls)
                {
                    MoveBall((int)ball.X, (int)ball.Y, false);
                }
            }
        }

        void HitBall(int x, int y)
        {
            poolState = PoolState.Moving;
            pnlTable.Cursor = Cursors.WaitCursor;
            double v = 20 *(progressBar1.Value / 100.0);
            progressBar1.Value = 0;
            double dx = x - balls[0].X + Ball.Radius;
            double dy = y - balls[0].Y + Ball.Radius;
            double h = (double)(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
            double sin = dy / h;
            double cos = dx / h;
            //balls[0].Velocity = velocity;
            balls[0].IsBallInHole = false;
            balls[0].XVelocity = v * cos;
            balls[0].YVelocity = v * sin;
            timer1.Enabled = true;

            using (StreamReader reader = new StreamReader(@"Shot01.wav"))
            {
                GCHandle myHandle = GCHandle.Alloc(reader.BaseStream, GCHandleType.Normal);
                SoundPlayerAsync.PlaySound(reader.BaseStream);
            }

            //SoundPlayer wavPlayer = new SoundPlayer();
            //wavPlayer.SoundLocation = @"Shot01.wav";
            //wavPlayer.LoadCompleted += new AsyncCompletedEventHandler(wavPlayer_LoadCompleted);
            //wavPlayer.LoadAsync();
        }

        private void wavPlayer_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ((System.Media.SoundPlayer)sender).Play();
            //((System.Media.SoundPlayer)sender).PlayLooping();
        }

        private void pnlTable_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (poolState == PoolState.AwaitingShot)
                    HitBall(e.X - pnlTable.Left + 4, e.Y - pnlTable.Top + 4);
            }            
        }

        private void pnlTable_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void frmTable_Activated(object sender, EventArgs e)
        {
            //if (tableGraphics == null)
            //{
                //tableGraphics = pnlTable.CreateGraphics();
                //foreach (Ball ball in balls)
                //{
                //    MoveBall((int)ball.X, (int)ball.Y, true);
                //}
            //}
        }

        private void frmTable_Deactivate(object sender, EventArgs e)
        {
            foreach (Ball ball in balls)
            {
                ball.IsStill = true;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //foreach (Ball ball in balls)
            //{
            //    ball.IsStill = true;
            //}

            //foreach (Ball ball in balls)
            //{
            //    MoveBall((int)ball.X, (int)ball.Y, true);
            //}
        }

        private void pnlTable_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                tableGraphics.DrawLine(new Pen(Brushes.White), new Point(e.X, e.Y), new Point((int)balls[0].X, (int)balls[0].Y));
        }

        private void ShowPoints()
        {
            txtPlayer1.Text = points1.ToString();
            txtPlayer2.Text = points2.ToString();
        }

        #region IHoleObserver Members

        public void AddPoints(int points)
        {
            points1 += points;
            txtPlayer1.Text = points1.ToString();
            txtPlayer2.Text = points2.ToString();
        }

        #endregion

        #region IHoleObserver Members

        public void BallDropped()
        {
            fallenBalls++;

            if (fallenBalls == balls.Count - 1)
            {
                timer1.Enabled = false;
                fallenBalls = 0;

                foreach (Ball ball in balls)
                {
                    ball.ResetPosition();
                }
                timer1.Enabled = true;
            }
        }

        #endregion
    }
}