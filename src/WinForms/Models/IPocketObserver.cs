using System;

namespace CSharpSnookerCore.Models
{
    public interface IPocketObserver
    {
        void BallDropped(Ball ball);
    }
}
