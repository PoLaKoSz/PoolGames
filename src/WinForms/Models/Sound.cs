using System;

namespace CSharpSnookerCore.Models
{
    public class Sound
    {
        public string Path { get; }
        public Vector2D Position { get; }



        public Sound(string path, Ball ball)
        {
            Path = path;
            Position = new Vector2D(ball.X, ball.Y);
        }



        protected static string PathGenerator(string fileName)
        {
            string folderName = "Resources\\Sounds";
            string extensionName = "wav";

            return string.Format("{0}\\{1}.{2}", folderName, fileName, extensionName);
        }
    }
}
