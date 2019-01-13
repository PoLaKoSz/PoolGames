using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnookerCore.Model
{
    public interface IBallObserver
    {
        void Hit(string sound);
    }
}
