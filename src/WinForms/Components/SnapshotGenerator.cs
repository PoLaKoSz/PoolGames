using CSharpSnookerCore.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace CSharpSnooker.WinForms.Components
{
    class SnapShotGenerator
    {
        public int SnapShotCount { get; private set; }
        public int VideoRefreshRate { get; }
        
        private int _maxSnapShot;
        private int _currentSnapShot;
        private List<BallPosition> _ballPositionList;

        private readonly Image _tableIMG;
        private readonly ImageAttributes _attr;
        private readonly PictureBox _pool;
        private readonly SoundManager _soundManager;
        private readonly Bitmap[] _whiteBitmapList;



        public SnapShotGenerator(PictureBox table, SoundManager soundManager)
        {
            _currentSnapShot = 1;
            this._pool = table;
            _soundManager = soundManager;
            VideoRefreshRate = 2;

            _tableIMG = Image.FromFile(@"Images\tableBlue.jpg");

            _attr = new ImageAttributes();
            _attr.SetColorKey(Color.White, Color.White);

            _ballPositionList = new List<BallPosition>();

            _whiteBitmapList = new Bitmap[300];

            for (int i = 0; i < _whiteBitmapList.Length; i++)
            {
                Bitmap bmp = new Bitmap(_tableIMG);

                _whiteBitmapList[i] = bmp;
            }
        }



        /// <summary>
        /// Plays all the generated snapshots.
        /// </summary>
        public void PlayLastShot()
        {
            DrawSnapShots();

            _currentSnapShot = 1;

            if (_ballPositionList.Count > 0)
            {
                _maxSnapShot = _ballPositionList[_ballPositionList.Count - 1].SnapShot + 1;
            }

            while (_currentSnapShot <= _maxSnapShot)
            {
                Thread.Sleep(VideoRefreshRate * 10);
                PlayCurrentSnapShot();
            }
        }

        public void DeleteSnapShots()
        {
            for (int i = 0; i < SnapShotCount; i++)
            {
                Bitmap bmp = new Bitmap(_tableIMG);

                _whiteBitmapList[i].Dispose();
                _whiteBitmapList[i] = bmp;
            }

            SnapShotCount = 0;

            _ballPositionList.Clear();
        }

        /// <summary>
        /// Adds the ball to the current frame.
        /// </summary>
        /// <param name="ball">Non null object.</param>
        public void AddToCurrentFrame(Ball ball)
        {
            _ballPositionList.Add(new BallPosition(SnapShotCount, ball.Image, (int)ball.X, (int)ball.Y));
        }

        /// <summary>
        /// Switch to the next frame.
        /// </summary>
        public void NextFrame()
        {
            SnapShotCount++;
        }


        /// <summary>
        /// Displys the current snapshot and plays the music.
        /// </summary>
        private void PlayCurrentSnapShot()
        {
            //Plays an individual frame, by replacing the image of the picturebox with
            //the stored image of a frame
            _pool.Image = _whiteBitmapList[_currentSnapShot - 1];
            _pool.Refresh();

            _soundManager.Play(_currentSnapShot - 1);

            _currentSnapShot++;
        }

        /// <summary>
        /// Generates images from the stored ball positions.
        /// </summary>
        private void DrawSnapShots()
        {
            int snapShot = -1;

            Graphics whiteBitmapGraphics = null;

            //For each ball, draws an image of that ball over the pool background image
            foreach (BallPosition ballPosition in _ballPositionList)
            {
                if (ballPosition.SnapShot != snapShot)
                {
                    snapShot = ballPosition.SnapShot;
                    whiteBitmapGraphics = Graphics.FromImage(_whiteBitmapList[snapShot]);
                }

                //draws an image of a ball over the pool background image
                whiteBitmapGraphics.DrawImage(ballPosition.Image, new Rectangle((int)(ballPosition.X - Ball.Radius), (int)(ballPosition.Y - Ball.Radius), (int)Ball.Radius * 2, (int)Ball.Radius * 2), 0, 0, (int)Ball.Radius * 2, (int)Ball.Radius * 2, GraphicsUnit.Pixel, _attr);
            }
        }
    }
}
