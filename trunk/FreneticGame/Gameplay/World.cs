using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public class World
    {
        List<Player> _players;
        public List<Player> Players
        {
            get
            {
                return _players;
            }
        }
        public World()
        {
            _players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }
    }
}
