using System;
using Microsoft.Xna.Framework;

namespace Frenetic
{
    public class KeyboardPlayerController : IController
    {
        public Player Player { get; private set; }
        public KeyboardPlayerController(Player player)
        {
            Player = player;
        }
        #region IController Members
        
        Random rand = new Random();
        int count = 0;
        public void Process()
        {
            if (count > 1000)
            {
                Player.Position = new Vector2(rand.Next(500), rand.Next(500));
                count = 0;
            }
            count++;
        }

        #endregion
    }
}
