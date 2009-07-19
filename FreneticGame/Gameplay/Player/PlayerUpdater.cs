using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Frenetic.Gameplay;

namespace Frenetic.Player
{
    public class PlayerUpdater : IController
    {
        public PlayerUpdater(List<IPlayer> playerList)
        {
            Players = playerList;
        }
        
        #region IController Members

        public void Process(float elapsedSeconds)
        {
            foreach (IPlayer player in Players)
            {
                if (player.PendingShot != null)
                {
                    Vector2 direction = (Vector2)player.PendingShot;
                    player.Shoot(direction);
                    player.PendingShot = null;
                }
            }
        }

        #endregion

        public List<IPlayer> Players { get; private set; }
    }
}
