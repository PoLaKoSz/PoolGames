using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnookerCore.Model
{
    public class Vector2D
    {
        #region attributes
        private double x;
        private double y;
        #endregion attributes

        #region constructor
        public Vector2D(double vx, double vy)
        {
            x = vx;
            y = vy;
        }

        public Vector2D(Vector2D v)
        {
            this.x = v.x;
            this.y = v.y;
        }
        #endregion constructor

        #region properties
        public double X
        {
            get { return x; }
            set
            {
                x = value;

                if (x > 15000)
                    value = value;
            }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }
        #endregion properties

        #region functions
        public Vector2D Add(Vector2D lhs, Vector2D rhs)
        {
            Vector2D result = new Vector2D(lhs);
            result.x += rhs.x;
            result.y += rhs.y;
            return (result);
        }

        public Vector2D Add(Vector2D v)
        {
            Vector2D result = new Vector2D(this);
            result.X += v.X;
            result.Y += v.Y;
            return (result);
        }

        public Vector2D Add(float f)
        {
            Vector2D result = new Vector2D(this);
            result.x += f;
            result.y += f;
            return (result);
        }

        public Vector2D Subtract(Vector2D lhs, Vector2D rhs)
        {
            Vector2D result = new Vector2D(lhs);
            result.x -= rhs.x;
            result.y -= rhs.y;
            return (result);
        }

        public Vector2D Subtract(Vector2D v)
        {
            Vector2D result = new Vector2D(this);
            result.X -= v.X;
            result.Y -= v.Y;
            return (result);
        }

        public Vector2D Multiply(double lhs, Vector2D rhs)
        {
            Vector2D result = new Vector2D(rhs);
            result.x *= lhs;
            result.y *= lhs;
            return (result);
        }

        public Vector2D Multiply(Vector2D lhs, double rhs)
        {
            Vector2D result = new Vector2D(lhs);
            result.x *= rhs;
            result.y *= rhs;
            return (result);
        }

        public Vector2D Multiply(double d)
        {
            Vector2D result = new Vector2D(this);
            result.x *= d;
            result.y *= d;
            return (result);
        }

        public float Lenght()
        {
            float ret = (float)(Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2)));
            return ret;
        }

        public float Dot(Vector2D v)
        {
            return ((float)(x * v.X + y * v.Y));
        }

        public Vector2D Normalize()
        {
            float l = Lenght();
            Vector2D result = new Vector2D(this);
            result.x = result.x / l;
            result.y = result.y / l;

            if (double.IsNaN(result.x) || double.IsNaN(result.y))
                l = l;
            return result;
        }
        #endregion functions
    } 
}
