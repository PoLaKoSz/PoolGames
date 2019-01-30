using CSharpSnooker.WinForms.Models.Events;
using CSharpSnookerCore.Models;
using System;

namespace CSharpSnooker.WinForms.Components
{
    class CollisionManager
    {
        public delegate void MyEventHandler(BallsCollisionEventArgs e);
        public event MyEventHandler OnBallsCollision;

        public void ResolveCollision(Ball ball1, Ball ball2)
        {
            // Get the mtd
            Vector2D delta = (ball1.Position.Subtract(ball2.Position));
            float d = delta.Lenght();

            // Minimum translation distance to push balls apart after intersecting
            Vector2D mtd = delta.Multiply((float)(((Ball.Radius + 1.0 + Ball.Radius + 1.0) - d) / d));

            // resolve intersection --
            // inverse mass quantities
            float im1 = 1f;
            float im2 = 1f;

            // Push-pull them apart based off their mass
            ball1.Position = ball1.Position.Add((mtd.Multiply(im1 / (im1 + im2))));
            ball2.Position = ball2.Position.Subtract(mtd.Multiply(im2 / (im1 + im2)));

            // Impact speed
            Vector2D v = (ball1.TranslateVelocity.Subtract(ball2.TranslateVelocity));
            float vn = v.Dot(mtd.Normalize());

            // Sphere intersecting but moving away from each other already
            if (vn > 0.0f)
                return;

            // Collision impulse
            float i = Math.Abs((float)((-(1.0f + 0.1) * vn) / (im1 + im2)));
            Vector2D impulse = mtd.Multiply(1);

            OnBallsCollision?.Invoke(new BallsCollisionEventArgs(ball1, impulse));

            // change in momentum
            ball1.TranslateVelocity = ball1.TranslateVelocity.Add(impulse.Multiply(im1));
            ball2.TranslateVelocity = ball2.TranslateVelocity.Subtract(impulse.Multiply(im2));
        }
    }
}
