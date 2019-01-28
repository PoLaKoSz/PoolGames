using CSharpSnookerCore.Models;
using System.Collections.Generic;

namespace CSharpSnooker.WinForms.Components
{
    class PlayerManager
    {
        public Player CurrentPlayer => GetCurrentPlayer();
        public Player OtherPlayer => GetOtherPlayer();


        private readonly Player _player1;
        private readonly Player _player2;

        private int _currentPlayerIndex;



        /// <summary>
        /// Initialize a new instance and sets player1 as the current.
        /// </summary>
        /// <param name="player1">Non null object.</param>
        /// <param name="player2">Non null object.</param>
        public PlayerManager(Player player1, Player player2)
        {
            _player1 = player1;
            _player2 = player2;
            _currentPlayerIndex = 0;
        }



        /// <summary>
        /// Changes the current player to the other one.
        /// </summary>
        /// <returns>Non null current Player object.</returns>
        public Player Switch()
        {
            if (_currentPlayerIndex == 0)
            {
                _currentPlayerIndex++;
            }
            else
            {
                _currentPlayerIndex = 0;
            }

            return CurrentPlayer;
        }

        /// <summary>
        /// Resets every data for both <see cref="Player"/>.
        /// </summary>
        public void EndMatch()
        {
            CurrentPlayer.FoulList = new List<int>();
            CurrentPlayer.JustSwapped = false;
            CurrentPlayer.BallOn = null;
            CurrentPlayer.Points = 0;

            OtherPlayer.FoulList = new List<int>();
            OtherPlayer.JustSwapped = false;
            OtherPlayer.BallOn = null;
            OtherPlayer.Points = 0;
        }


        private Player GetCurrentPlayer()
        {
            if (_currentPlayerIndex == 0)
            {
                return _player1;
            }
            else
            {
                return _player2;
            }
        }

        private Player GetOtherPlayer()
        {
            if (_currentPlayerIndex == 0)
            {
                return _player2;
            }
            else
            {
                return _player1;
            }
        }
    }
}
