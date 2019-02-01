using CSharpSnooker.WinForms.Models.Events;
using CSharpSnookerCore.Models;
using System;
using System.Collections.Generic;

namespace CSharpSnooker.WinForms.Components
{
    class PocketManager
    {
        public List<Pocket> Pockets { get; }
        public delegate void MyEventHandler(BallPottedEventArgs e);
        public event MyEventHandler OnPotting;



        public PocketManager()
        {
            Pockets = new List<Pocket>()
            {
                new Pocket(1, 5, 5, 29, 29),
                new Pocket(2, 288, 0, 301, 25),
                new Pocket(3, 571, 5, 573, 29),
                new Pocket(4, 5, 309, 29, 309),
                new Pocket(5, 288, 314, 301, 313),
                new Pocket(6, 571, 309, 572, 310),
            };
        }



        /// <summary>
        /// Raises a <see cref="BallPottedEventArgs"/> if the ball is in the pocket.
        /// </summary>
        /// <param name="ball">The moving ball.</param>
        /// <param name="pocket">The ball destination pocket.</param>
        public void DetectPotting(Ball ball, Pocket pocket)
        {
            double xd = (pocket.X - ball.X);
            double yd = (pocket.Y - ball.Y);

            double sumRadius = (Ball.Radius * 1.5);
            double sqrRadius = sumRadius * sumRadius;

            double distSqr = (xd * xd) + (yd * yd);

            if (Math.Round(distSqr) > Math.Round(sqrRadius))
            {
                return;
            }

            if (!ball.IsInPocket)
            {
                OnPotting?.Invoke(new BallPottedEventArgs(ball, pocket));
            }

            if (ball.Position.X != ball.LastX || ball.Position.Y != ball.LastY)
                ball.IsInPocket = true;

            ball.Position.X = pocket.X;
            ball.Position.Y = pocket.Y;

            ball.Velocity.X = 0;
            ball.Velocity.Y = 0;
        }
    }
}
