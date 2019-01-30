using CSharpSnookerCore.Models;

namespace CSharpSnooker.WinForms.Components.PoolTypes
{
    interface IPoolType
    {
        Ball[] Balls { get; }

        /// <summary>
        /// If it's true, than somebody won the game.
        /// </summary>
        bool HasWinner { get; }


        void InitBallOn(PlayerManager playerManager);

        void ProcessFallenBalls(BallManager ballManager, PlayerManager playerManager);
    }
}
