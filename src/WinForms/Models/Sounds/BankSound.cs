using System;

namespace CSharpSnookerCore.Models.Sounds
{
    public class BankSound : Sound
    {
        private static readonly string FilePath;



        static BankSound()
        {
            FilePath = PathGenerator("Bank");
        }

        public BankSound(Ball ball)
            : base(FilePath, ball) { }
    }
}
