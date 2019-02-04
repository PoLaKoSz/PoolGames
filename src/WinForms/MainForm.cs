using CSharpSnooker.WinForms.Components;
using CSharpSnookerCore.Models;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CSharpSnooker.WinForms
{
    partial class MainForm : Form
    {
        public PlayerManager PlayerManager { get; set; }
        public Vector2D CueBallSpinVector { get; private set; }

        private bool _showBallOn = true;
        private Graphics _tableGraphics;

        private readonly GameEngine _viewModel;
        private readonly Rectangle _thermometerRectangle;
        private readonly Rectangle _targetRectangle;
        private readonly Image _imgQuestionBall;



        public MainForm(GameEngine viewModel, PlayerManager playerManager)
        {
            _viewModel = viewModel;
            _thermometerRectangle = new Rectangle(32, 249, 152, 10);
            _targetRectangle      = new Rectangle(68, 269, 71, 71);
            PlayerManager = playerManager;

            InitializeComponent();

            if (PlayerManager.CurrentPlayer.IsComputer)
            {
                lblPlayer1Name.Text = PlayerManager.CurrentPlayer.Name;
                lblPlayer2Name.Text = PlayerManager.OtherPlayer.Name;
            }
            else
            {
                lblPlayer1Name.Text = PlayerManager.OtherPlayer.Name;
                lblPlayer2Name.Text = PlayerManager.CurrentPlayer.Name;
            }

            _imgQuestionBall = Image.FromFile(@"Images\questionball.PNG");

            lblStrenght.Width = (int)((PlayerManager.CurrentPlayer.Strength * (_thermometerRectangle.Width - 12) / 100.0));

            timerInBox.Enabled = PlayerManager.CurrentPlayer.IsComputer;

            CueBallSpinVector = new Vector2D(0, 0);
        }



        public void InitializePoolTable()
        {
            _tableGraphics = picTable.CreateGraphics();
        }

        public void ShowPoints(Player player1, Player player2)
        {
            lblPlayer1.Text = player1.Points.ToString();
            lblPlayer2.Text = player2.Points.ToString();
        }

        public void SetBallOnImage(Ball ball)
        {
            if (ball != null)
            {
                picBallOn.Image = ball.Image;
            }
            else
            {
                picBallOn.Image = _imgQuestionBall;
            }
        }

        public void ShowWinner()
        {
            if (PlayerManager.CurrentPlayer.Points == PlayerManager.OtherPlayer.Points)
            {
                lblWin.Text = "Draw!";
            }
            else if (PlayerManager.OtherPlayer.Points < PlayerManager.CurrentPlayer.Points)
            {
                lblWin.Text = string.Format("{0} Wins!", PlayerManager.CurrentPlayer.Name);
            }
            else
            {
                lblWin.Text = string.Format("{0} Wins!", PlayerManager.OtherPlayer.Name);
            }

            picTable.Image = Image.FromFile(@"Images\tableBlue.JPG");

            picTable.Update();

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

            lblWin.Visible = false;
        }

        public void ResetCueBallSpinIndicator()
        {
            int ballX = (_targetRectangle.X + _targetRectangle.Width / 2);
            int ballY = (_targetRectangle.Y + _targetRectangle.Height / 2);

            lblTarget.Location = new Point(ballX - 3, ballY - 3);

            CueBallSpinVector = new Vector2D(0, 0);
        }

        public void CallingBall()
        {
            ChangeCursor(Cursors.Hand);
        }

        public void HittingBall()
        {
            ChangeCursor(Cursors.WaitCursor);
        }

        public void AimWithCueBall()
        {
            ChangeCursor(Cursors.SizeAll);
        }

        public void PoolTable_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _viewModel.PoolTable_MouseUp(e);
        }

        public void PoolTable_MouseMove(object sender, MouseEventArgs e)
        {
            _viewModel.PoolTable_MouseMove(e);
        }


        private void BallOnTimer_Tick(object sender, EventArgs e)
        {
            picBallOn.Top = 90 + (PlayerManager.CurrentPlayer.Id - 1) * 58;
            _showBallOn = !_showBallOn;
            picBallOn.Visible = _showBallOn;
        }

        private bool SetStrength(int x, int y)
        {
            if (x >= _thermometerRectangle.X && x <= _thermometerRectangle.X + _thermometerRectangle.Width && y >= _thermometerRectangle.Y - 3 && y <= _thermometerRectangle.Y + _thermometerRectangle.Height + 3)
            {
                lblStrenght.Width = x - _thermometerRectangle.Left - 6;

                if (!PlayerManager.CurrentPlayer.IsComputer)
                {
                    PlayerManager.CurrentPlayer.Strength = (int)(1.0 * lblStrenght.Width / (_thermometerRectangle.Width - 12) * 100.0);
                }
                else
                {
                    PlayerManager.OtherPlayer.Strength = (int)(1.0 * lblStrenght.Width / (_thermometerRectangle.Width - 12) * 100.0);
                }

                return true;
            }

            return false;
        }

        private Vector2D SetTarget(int x, int y)
        {
            Vector2D vector = CueBallSpinVector;

            int ballX = (int)(_targetRectangle.X + _targetRectangle.Width / 2.0);
            int ballY = (int)(_targetRectangle.Y + _targetRectangle.Height / 2.0);
            int ballRadius = _targetRectangle.Width / 2;

            double xd = (x - ballX);
            double yd = (y - ballY);

            double sumRadius = (_targetRectangle.Width / 2);
            double sqrRadius = sumRadius * sumRadius;

            double distSqr = (xd * xd) + (yd * yd);

            if (Math.Round(distSqr) < Math.Round(sqrRadius))
            {
                vector = new Vector2D(xd, yd);
            }

            return vector;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X >= _thermometerRectangle.X && e.X <= _thermometerRectangle.X + _thermometerRectangle.Width && e.Y >= _thermometerRectangle.Y - 3 && e.Y <= _thermometerRectangle.Y + _thermometerRectangle.Height + 3)
            {
                CallingBall();
            }
            else
            {
                int ballX = (_targetRectangle.X + _targetRectangle.Width / 2);
                int ballY = (_targetRectangle.Y + _targetRectangle.Height / 2);
                int ballRadius = _targetRectangle.Width / 2;

                double xd = (e.X - ballX);
                double yd = (e.Y - ballY);

                double sqrRadius = ballRadius * ballRadius;

                double distSqr = (xd * xd) + (yd * yd);

                if (Math.Round(distSqr) < Math.Round(sqrRadius))
                {
                    CallingBall();
                }
                else
                {
                    ChangeCursor(Cursors.Default);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            TrySetTargetOrStrenght(e.X, e.Y);
        }

        private void TrySetTargetOrStrenght(int x, int y)
        {
            if (!SetStrength(x, y))
            {
                CueBallSpinVector = SetTarget(x, y);
                lblTarget.Location = new Point((int)(_targetRectangle.X + _targetRectangle.Width / 2 + CueBallSpinVector.X - 3), (int)(_targetRectangle.Y + _targetRectangle.Height / 2 + CueBallSpinVector.Y - 3));
            }
        }

        private void lblStrenght_MouseDown(object sender, MouseEventArgs e)
        {
            SetStrength(lblStrenght.Left + e.X, lblStrenght.Top + e.Y);
        }

        private void ChangeCursor(Cursor cursor)
        {
            picTable.Cursor = cursor;
        }
    }
}