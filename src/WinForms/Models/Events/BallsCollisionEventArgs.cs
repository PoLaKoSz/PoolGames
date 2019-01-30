using CSharpSnookerCore.Models;
using System;

namespace CSharpSnooker.WinForms.Models.Events
{
    class BallsCollisionEventArgs : EventArgs
    {
        public Ball Ball { get; }
        public Vector2D Impulse { get; }



        public BallsCollisionEventArgs(Ball ball, Vector2D impulse)
        {
            Ball = ball;
            Impulse = impulse;
        }
    }
}
