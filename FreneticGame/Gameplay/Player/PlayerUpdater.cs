using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Frenetic.Player
{
    public class PlayerUpdater : IController
    {
        public PlayerUpdater()
        {
            Players = new List<IPlayer>();
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

        public void AddPlayer(IPlayer player)
        {
            Players.Add(player);
        }
        public void RemovePlayer(IPlayer player)
        {
            Players.Remove(player);
        }
        public List<IPlayer> Players { get; private set; }

    }
}
