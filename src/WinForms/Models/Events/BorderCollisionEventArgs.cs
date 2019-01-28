using CSharpSnookerCore.Models;
using System;

namespace CSharpSnooker.WinForms.Models.Events
{
    class BorderCollisionEventArgs : EventArgs
    {
        public Ball Ball { get; }



        public BorderCollisionEventArgs(Ball ball)
        {
            Ball = ball;
        }
    }
}
