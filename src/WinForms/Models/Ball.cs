using System;
using System.Collections.Generic;
using System.Drawing;

namespace CSharpSnookerCore.Models
{
    public class Ball
    {
        public static readonly double Radius;
        public static readonly int CosBall45;
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
        Image image;
        int points;
        bool isBallInPocket = false;



        static Ball()
        {
            Radius = 8;
            CosBall45 = (int)(Math.Cos(Math.PI / 4) * Radius);
        }

        public Ball(string id, int x, int y, Image image, int points)
        {
            this.id = id;
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



        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public bool IsInPocket
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

        public override string ToString()
        {
            return string.Format("Ball({0}, {1})", (int)position.X, (int)position.Y);
        }

        public override bool Equals(object obj)
        {
            return ((Ball)obj).id.Equals(id);
        }

        public override int GetHashCode()
        {
            var hashCode = -1651333200;
            hashCode = hashCode * -1521134295 + isStill.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(initPosition);
            hashCode = hashCode * -1521134295 + lastX.GetHashCode();
            hashCode = hashCode * -1521134295 + lastY.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(position);
            hashCode = hashCode * -1521134295 + width.GetHashCode();
            hashCode = hashCode * -1521134295 + height.GetHashCode();
            hashCode = hashCode * -1521134295 + rad.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(translateVelocity);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(vSpinVelocity);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(hSpinVelocity);
            hashCode = hashCode * -1521134295 + EqualityComparer<Image>.Default.GetHashCode(image);
            hashCode = hashCode * -1521134295 + points.GetHashCode();
            hashCode = hashCode * -1521134295 + isBallInPocket.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + IsInPocket.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Image>.Default.GetHashCode(Image);
            hashCode = hashCode * -1521134295 + Points.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(Position);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(InitPosition);
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + LastX.GetHashCode();
            hashCode = hashCode * -1521134295 + LastY.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            hashCode = hashCode * -1521134295 + Rad.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(Velocity);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(TranslateVelocity);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(VSpinVelocity);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(HSpinVelocity);
            hashCode = hashCode * -1521134295 + IsStill.GetHashCode();
            return hashCode;
        }
    }
}

