using CSharpSnookerCore.Models;

namespace CSharpSnooker.WinForms.Components
{
    class PlayerViewModel
    {
        public Player Model { get; }



        public PlayerViewModel(Player player)
        {
            Model = player;
        }



        public virtual Vector2D GiveControl(PlayerManager playerManager, Simulator simulator)
        {
            return null;
        }
    }
}
