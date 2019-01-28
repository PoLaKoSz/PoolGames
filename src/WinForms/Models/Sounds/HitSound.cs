using System;

namespace CSharpSnookerCore.Models.Sounds
{
    public class HitSound : Sound
    {
        public HitSound(string volume, Ball ball)
            : base(PathGenerator("Hit" + volume), ball) { }
    }
}
