using CSharpSnookerCore.Models;
using System.Collections.Generic;

namespace CSharpSnooker.WinForms.Components.PoolTypes
{
    interface IPoolType
    {
        Ball[] Balls { get; }

        /// <summary>
        /// If it's true, than somebody won the game.
        /// </summary>
        bool HasWinner { get; }


        void InitBallOn(PlayerManager playerManager, BallManager ballManager);

        void ProcessFallenBalls(BallManager ballManager, PlayerManager playerManager);

        /// <summary>
        /// Returns all balls that the current player can shot down.
        /// </summary>
        /// <param name="player">Non null object.</param>
        /// <returns>Non null object.</returns>
        List<Ball> PottableBalls(Player player);
    }
}
