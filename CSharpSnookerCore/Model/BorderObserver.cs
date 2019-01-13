using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnookerCore.Model
{
    public interface IBorderObserver
    {
        void Hit(string sound);
    }
}
