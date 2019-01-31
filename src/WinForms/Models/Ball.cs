using System;
using System.Collections.Generic;
using System.Drawing;

namespace CSharpSnookerCore.Models
{
    public class Ball
    {
        public bool IsInPocket { get; set; }
        public bool IsStill { get; set; }
        public Vector2D Position { get; set; }
        public Vector2D Velocity { get; set; }
        public Vector2D VSpinVelocity { get; set; }
        public double LastX { get; set; }
        public double LastY { get; set; }

        public double X
        {
            get { return Position.X; }
            set
            {
                Position.X = value;
                IsStill = false;
            }
        }
        public double Y
        {
            get { return Position.Y; }
            set
            {
                Position.Y = value;
                IsStill = false;
            }
        }

        public string Id { get; }
        public Image Image { get; }
        public int Points { get; }
        public Vector2D InitPosition { get; }
        
        public static readonly double Radius;
        public static readonly int CosBall45;



        static Ball()
        {
            Radius    = 8;
            CosBall45 = (int)(Math.Cos(Math.PI / 4) * Radius);
        }

        public Ball(string id, int x, int y, Image image, int points)
        {
            Id            = id;
            Position.X    = x;
            Position.Y    = y;
            LastX         = x;
            LastY         = y;
            InitPosition  = new Vector2D(x, y);
            Position      = new Vector2D(0, 0);
            Velocity      = new Vector2D(0, 0);
            VSpinVelocity = new Vector2D(0, 0);
            Image         = image;
            Points        = points;
            IsStill       = true;
        }



        public void ResetPositionAt(double x, double y)
        {
            Velocity   = new Vector2D(0, 0);
            IsInPocket = false;
            Position.X = x;
            Position.Y = y;
            LastX      = x;
            LastY      = y;
        }


        public override string ToString()
        {
            return string.Format("Ball({0}, {1}) - {2} points", (int)Position.X, (int)Position.Y, Points);
        }

        public override bool Equals(object obj)
        {
            return ((Ball)obj).Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            var hashCode = -1651333200;
            hashCode = hashCode * -1521134295 + IsStill.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(InitPosition);
            hashCode = hashCode * -1521134295 + LastX.GetHashCode();
            hashCode = hashCode * -1521134295 + LastY.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(Position);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(VSpinVelocity);
            hashCode = hashCode * -1521134295 + EqualityComparer<Image>.Default.GetHashCode(Image);
            hashCode = hashCode * -1521134295 + Points.GetHashCode();
            hashCode = hashCode * -1521134295 + IsInPocket.GetHashCode();
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
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(Velocity);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(VSpinVelocity);
            hashCode = hashCode * -1521134295 + IsStill.GetHashCode();
            return hashCode;
        }
    }
}

