using System;
using Microsoft.Xna.Framework;

namespace Frenetic
{
    public class KeyboardPlayerController : IController
    {
        public IPlayer Player { get; private set; }
        public KeyboardPlayerController(IPlayer player)
        {
            Player = player;
        }
        #region IController Members
        
        Random rand = new Random();
        int count = 0;
        public void Process()
        {
            if (count > 300)
            {
                Player.Position = new Vector2(rand.Next(800), rand.Next(600));
                count = 0;
            }
            count++;

            Player.Update();
        }

        #endregion
    }
}
