using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Media;

namespace CSharpSnookerCore.Model
{
    [Serializable]
    public class Ball
    {
        #region attributes
        public static double Radius = 8;
        public static int CosBall45;
        bool isStill = true;
        string id;
        Vector2D initPosition = new Vector2D(0, 0);
        double lastX;
        double lastY;
        Vector2D position = new Vector2D(0, 0);
        int width;
        int height;
        double rad = 0;
        Vector2D translateVelocity = new Vector2D(0, 0);
        Vector2D vSpinVelocity = new Vector2D(0, 0);
        Vector2D hSpinVelocity = new Vector2D(0, 0);
        IBallObserver observer;
        SoundPlayer myPlayer;
        Image image;
        int points;
        bool isBallInPocket = false;
        string soundLocation;
        string pocketSound;
        static StreamReader reader;
        static Stream pocketSoundSteam;
        #endregion attributes

        #region constructor

        public Ball(string id, IBallObserver observer, int x, int y, string soundLocation, string pocketSound, Image image, int points)
        {
            CosBall45 = (int)(Math.Cos(Math.PI / 4) * Ball.Radius);
            this.id = id;
            this.observer = observer;
            width = 32;
            height = 32;
            this.initPosition = new Vector2D(x, y);
            this.position.X = x;
            this.position.Y = y;
            lastX = x;
            lastY = y;
            this.image = image;
            this.points = points;
        }

        #endregion constructor

        #region properties
        
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public bool IsBallInPocket
        {
            get { return isBallInPocket; }
            set {
                if (isBallInPocket == false && value)
                {
                    if (value && id == "01")
                    {
                        isBallInPocket = false;
                        position.X = initPosition.X;
                        position.Y = initPosition.Y;
                        return;
                    }
                }

                isBallInPocket = value;

            }
        }

        public Image Image
        {
            get { return image; }
        }

        public int Points
        {
            get { return points; }
        }

        public Vector2D Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2D InitPosition
        {
            get { return initPosition; }
            set { initPosition = value; }
        }

        public double X
        {
            get { return position.X; }
            set
            {
                position.X = value;
                isStill = false;
            }
        }

        public double Y
        {
            get { return position.Y; }
            set
            {
                position.Y = value;
                isStill = false;
            }
        }

        public double LastX
        {
            get { return lastX; }
            set { lastX = value; }
        }

        public double LastY
        {
            get { return lastY; }
            set { lastY = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public double Rad
        {
            get { return rad; }
            set { rad = value; }
        }

        private Vector2D Velocity
        {
            get { return translateVelocity; }
        }

        public Vector2D TranslateVelocity
        {
            get { return translateVelocity; }
            set { translateVelocity = value; }
        }

        public Vector2D VSpinVelocity
        {
            get { return vSpinVelocity; }
            set { vSpinVelocity = value; }
        }

        public Vector2D HSpinVelocity
        {
            get { return hSpinVelocity; }
            set { hSpinVelocity = value; }
        }

        public bool IsStill
        {
            get { return isStill; }
            set { isStill = value; }
        }

        public void ResetPosition()
        {
            translateVelocity = new Vector2D(0, 0);
            isBallInPocket = false;
            position.X = initPosition.X;
            position.Y = initPosition.Y;
            lastX = position.X;
            lastY = position.Y;
        }

        #endregion properties

        #region functions

        public void ResetPositionAt(double x, double y)
        {
            translateVelocity = new Vector2D(0, 0);
            isBallInPocket = false;
            position.X = x;
            position.Y = y;
            lastX = x;
            lastY = y;
        }

        public void SetPosition(double x, double y)
        {
            this.position.X = x;
            this.position.Y = y;

            lastX = x;
            LastY = y;
            isStill = false;
        }

        public bool Colliding(Ball ball)
        {
            if (!ball.isBallInPocket && !isBallInPocket)
            {
                float xd = (float)(position.X - ball.X);
                float yd = (float)(position.Y - ball.Y);

                float sumRadius = (float)((Ball.Radius + 1.0) * 2);
                float sqrRadius = sumRadius * sumRadius;

                float distSqr = (xd * xd) + (yd * yd);

                if (Math.Round(distSqr) < Math.Round(sqrRadius))
                {
                    return true;
                }
            }

            return false;
        }

        public void ResolveCollision(Ball ball)
        {
            // get the mtd
            Vector2D delta = (position.Subtract(ball.position));
            float d = delta.Lenght();
            // minimum translation distance to push balls apart after intersecting
            Vector2D mtd = delta.Multiply((float)(((Ball.Radius + 1.0 + Ball.Radius + 1.0) - d) / d));

            // resolve intersection --
            // inverse mass quantities
            float im1 = 1f;
            float im2 = 1f;

            // push-pull them apart based off their mass
            position = position.Add((mtd.Multiply(im1 / (im1 + im2))));
            ball.position = ball.position.Subtract(mtd.Multiply(im2 / (im1 + im2)));

            // impact speed
            Vector2D v = (this.translateVelocity.Subtract(ball.translateVelocity));
            float vn = v.Dot(mtd.Normalize());

            // sphere intersecting but moving away from each other already
            if (vn > 0.0f)
                return;

            // collision impulse
            float i = Math.Abs((float)((-(1.0f + 0.1) * vn) / (im1 + im2)));
            Vector2D impulse = mtd.Multiply(1);

            int hitSoundIntensity = (int)(Math.Abs(impulse.X) + Math.Abs(impulse.Y));

            if (hitSoundIntensity > 5)
                hitSoundIntensity = 5;

            if (hitSoundIntensity < 1)
                hitSoundIntensity = 1;

            double xSound = (float)(ball.Position.X - 300.0) / 300.0;
            observer.Hit(string.Format(@"Sounds\Hit{0}.wav", hitSoundIntensity.ToString("00")) + "|" + xSound.ToString());

            // change in momentum
            this.translateVelocity = this.translateVelocity.Add(impulse.Multiply(im1));
            ball.translateVelocity = ball.translateVelocity.Subtract(impulse.Multiply(im2));
        }

        private void wavPlayer_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ((System.Media.SoundPlayer)sender).Play();
        }

        public override string ToString()
        {
            return string.Format("Ball({0}, {1})", (int)position.X, (int)position.Y);
        }

        public override bool Equals(object obj)
        {
            return ((Ball)obj).id.Equals(id);
        }

        #endregion functions
    }
}

