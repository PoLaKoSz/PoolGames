using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnookerCore.Model
{
    public interface IPocketObserver
    {
        void BallDropped(Ball ball);
    }
}
