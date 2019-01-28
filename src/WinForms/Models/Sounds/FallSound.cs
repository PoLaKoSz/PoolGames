using System;

namespace CSharpSnookerCore.Models.Sounds
{
    public class FallSound : Sound
    {
        private static readonly string FilePath;



        static FallSound()
        {
            FilePath = PathGenerator("Fall");
        }

        public FallSound(Ball ball)
            : base(FilePath, ball) { }
    }
}
