using System;
using Microsoft.Xna.Framework;
namespace Frenetic.Player
{
    public interface IPlayerInput
    {
        void RefreshPlayerValuesFromInput(IPlayer player);

        PlayerStatus? PendingStatus { get; set; }
        Vector2? PendingShot { get; set; }
        Vector2 Position { get; set; }
    }
}
