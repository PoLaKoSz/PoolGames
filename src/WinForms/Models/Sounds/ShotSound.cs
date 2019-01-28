using System;

namespace CSharpSnookerCore.Models.Sounds
{
    public class ShotSound : Sound
    {
        private static readonly string FilePath;



        static ShotSound()
        {
            FilePath = PathGenerator("Shot");
        }

        public ShotSound(Ball ball)
            : base(FilePath, ball) { }
    }
}
