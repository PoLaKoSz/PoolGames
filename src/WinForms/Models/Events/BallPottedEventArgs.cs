using CSharpSnookerCore.Models;
using System;

namespace CSharpSnooker.WinForms.Models.Events
{
    class BallPottedEventArgs : EventArgs
    {
        public Ball Ball { get; }
        public Pocket Pocket { get; }



        public BallPottedEventArgs(Ball ball, Pocket pocket)
        {
            Ball = ball;
            Pocket = pocket;
        }
    }
}
