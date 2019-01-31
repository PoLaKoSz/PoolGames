using CSharpSnookerCore.Models;

namespace CSharpSnooker.WinForms.Components
{
    class ComputerViewModel : PlayerViewModel
    {
        private readonly ComputerShotGenerator _computerShotGenerator;



        public ComputerViewModel(Player player, ComputerShotGenerator computerShotGenerator)
            : base(player)
        {
            _computerShotGenerator = computerShotGenerator;
        }



        public override Vector2D GiveControl(PlayerManager playerManager, Simulator simulator)
        {
            var shotVector = _computerShotGenerator.GenerateComputerShot(playerManager, simulator);

            return shotVector;
        }
    }
}
