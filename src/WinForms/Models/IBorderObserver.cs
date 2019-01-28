using System;

namespace CSharpSnookerCore.Models
{
    public interface IBorderObserver
    {
        void WallCollision(Ball ball);
    }
}
