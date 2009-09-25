using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Frenetic.Gameplay.Level;

namespace Frenetic.Player
{
    public class PlayerUpdater : IController
    {
        public PlayerUpdater(List<IPlayer> playerList)
        {
            this.Players = playerList;
        }
        
        #region IController Members

        public void Process(float elapsedSeconds)
        {
            foreach (IPlayer player in Players)
            {
                if (player.Status == PlayerStatus.Alive)
                {
                    if (player.PendingShot != null)
                    {
                        Vector2 direction = (Vector2)player.PendingShot;
                        player.Shoot(direction);
                        player.PendingShot = null;
                    }
                }
                else if (player.PendingStatus == PlayerStatus.Alive)
                {
                    // Player has been respawned on Client side...
                    player.Status = PlayerStatus.Alive;
                    player.Health = BasePlayer.StartHealth;
                }
            }
        }

        #endregion

        public List<IPlayer> Players { get; private set; }
    }
}
