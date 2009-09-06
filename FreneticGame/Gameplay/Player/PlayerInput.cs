using Microsoft.Xna.Framework;

namespace Frenetic.Player
{
    public class PlayerInput : IPlayerInput
    {
        public Vector2 Position { get; set; }
        public Vector2? PendingShot { get; set; }
    }
}
