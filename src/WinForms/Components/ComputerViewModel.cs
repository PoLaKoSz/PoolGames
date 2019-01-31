using CSharpSnookerCore.Models;
using System;

namespace CSharpSnooker.WinForms.Components
{
    class ComputerViewModel : PlayerViewModel
    {
        private readonly BallManager _ballManager;
        private readonly PocketManager _pocketManager;

        private static readonly Random _random;



        static ComputerViewModel()
        {
            _random = new Random();
        }

        public ComputerViewModel(Player player, BallManager ballManager, PocketManager pocketManager)
            : base(player)
        {
            Model.Strength = GetRandomStrenght();
            _ballManager = ballManager;
            _pocketManager = pocketManager;
        }



        public override Vector2D Hitting()
        {
            return new Vector2D(_random.Next(0, 600), _random.Next(1, 340));
        }

        private int GetRandomStrenght()
        {
            return _random.Next(20) + 30;
        }
    }
}
