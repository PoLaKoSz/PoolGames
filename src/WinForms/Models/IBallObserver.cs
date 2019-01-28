using System;

namespace CSharpSnookerCore.Models
{
    public interface IBallObserver
    {
        void Hit(string sound, Ball ball);
    }
}
