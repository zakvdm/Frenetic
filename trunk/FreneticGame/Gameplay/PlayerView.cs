using System;

namespace Frenetic
{
    public class PlayerView : IView
    {
        private Player _player;
        public PlayerView(Player player)
        {
            _player = player;
        }
        #region IView Members

        int count = 0;
        public void Generate()
        {
            if (count > 100)
            {
                Console.WriteLine("CLIENT: Player " + _player.ID.ToString() + " position is: " + _player.Position.ToString());
                count = 0;
            }
            count++;
        }

        #endregion
    }
}
