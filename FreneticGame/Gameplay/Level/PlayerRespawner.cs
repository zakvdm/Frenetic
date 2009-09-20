using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Player;
using Microsoft.Xna.Framework;

namespace Frenetic.Gameplay.Level
{
    public class PlayerRespawner : IPlayerRespawner
    {
        public void RespawnPlayer(IPlayer player)
        {
            if (player.PendingStatus != PlayerStatus.Alive)
            {
                player.PendingStatus = PlayerStatus.Alive;
                player.Position = new Vector2(400, 100);
            }
        }
    }
}
