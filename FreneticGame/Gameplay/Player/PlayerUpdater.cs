using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Frenetic.Gameplay.Level;

namespace Frenetic.Player
{
    public class PlayerUpdater : IController
    {
        public PlayerUpdater(List<IPlayer> playerList, IPlayerRespawner playerRespawner)
        {
            this.Players = playerList;
            this.PlayerRespawner = playerRespawner;
        }
        
        #region IController Members

        public void Process(float elapsedSeconds)
        {
            foreach (IPlayer player in Players)
            {
                if (player.PendingShot != null)
                {
                    if (player.IsAlive)
                    {
                        Vector2 direction = (Vector2)player.PendingShot;
                        player.Shoot(direction);
                        player.PendingShot = null;
                    }
                    else
                    {
                        this.PlayerRespawner.RespawnPlayer(player);
                    }
                }
            }
        }

        #endregion

        public List<IPlayer> Players { get; private set; }
        IPlayerRespawner PlayerRespawner;
    }
}
