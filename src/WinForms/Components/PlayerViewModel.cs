using CSharpSnookerCore.Models;
using System;

namespace CSharpSnooker.WinForms.Components
{
    class PlayerViewModel
    {
        public Player Model { get; }



        public PlayerViewModel(Player player)
        {
            Model = player;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Cue ball direction (coordinates of the mouse).</param>
        /// <param name="cueBall">Actual cue ball object.</param>
        /// <param name="targetVector">Spin value on the cue ball.</param>
        public void HitBall(Vector2D position, Ball cueBall, Vector2D targetVector)
        {
            // 20 is the maximum velocity
            double v = 20 * (Model.Strength / 100.0);

            // Calculates the cue angle, and the translate velocity (normal velocity)
            double deltaX = position.X - cueBall.X;
            double deltaY = position.Y - cueBall.Y;

            double h = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            double sin = deltaY / h;
            double cos = deltaX / h;

            cueBall.IsInPocket = false;
            cueBall.TranslateVelocity.X = v * cos;
            cueBall.TranslateVelocity.Y = v * sin;

            Vector2D normalVelocity = cueBall.TranslateVelocity.Normalize();

            // Calculates the top spin/back spin velocity, in the same direction as the normal velocity, but in opposite angle
            double topBottomVelocityRatio = cueBall.TranslateVelocity.Lenght() * (targetVector.Y / 100.0);
            cueBall.VSpinVelocity = new Vector2D(-1.0d * topBottomVelocityRatio * normalVelocity.X, -1.0d * topBottomVelocityRatio * normalVelocity.Y);

            Model.ShotCount++;
        }

        public virtual Vector2D Hitting()
        {
            return null;
        }

        public virtual void GiveControl() { }
    }
}
