using Microsoft.Xna.Framework;

namespace Frenetic.Player
{
    public class PlayerInput : IPlayerInput
    {
        public PlayerInput() { }
        public PlayerInput(IPlayer player)
        {
            this.Position = player.Position;
            this.PendingShot = player.PendingShot;
        }

        public void RefreshPlayerValuesFromInput(IPlayer player)
        {
            player.PendingShot = this.PendingShot;
            player.Position = this.Position;
        }

        public Vector2 Position { get; set; }
        public Vector2? PendingShot { get; set; }
    }
}
