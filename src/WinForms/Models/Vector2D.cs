using System;

namespace CSharpSnookerCore.Models
{
    public class Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }



        public Vector2D(double vx, double vy)
        {
            X = vx;
            Y = vy;
        }

        public Vector2D(Vector2D v)
        {
            this.X = v.X;
            this.Y = v.Y;
        }



        public Vector2D Add(Vector2D lhs, Vector2D rhs)
        {
            Vector2D result = new Vector2D(lhs);
            result.X += rhs.X;
            result.Y += rhs.Y;
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
            result.X += f;
            result.Y += f;
            return (result);
        }

        public Vector2D Subtract(Vector2D lhs, Vector2D rhs)
        {
            Vector2D result = new Vector2D(lhs);
            result.X -= rhs.X;
            result.Y -= rhs.Y;
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
            result.X *= lhs;
            result.Y *= lhs;
            return (result);
        }

        public Vector2D Multiply(Vector2D lhs, double rhs)
        {
            Vector2D result = new Vector2D(lhs);
            result.X *= rhs;
            result.Y *= rhs;
            return (result);
        }

        public Vector2D Multiply(double d)
        {
            Vector2D result = new Vector2D(this);
            result.X *= d;
            result.Y *= d;
            return (result);
        }

        public float Lenght()
        {
            float ret = (float)(Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2)));
            return ret;
        }

        public float Dot(Vector2D v)
        {
            return ((float)(X * v.X + Y * v.Y));
        }

        public Vector2D Normalize()
        {
            float l = Lenght();

            Vector2D result = new Vector2D(this);
            result.X = result.X / l;
            result.Y = result.Y / l;

            return result;
        }
    } 
}
