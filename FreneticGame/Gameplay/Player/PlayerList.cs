using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Player
{
    public interface IPlayerList
    {
        event Action<IPlayer> PlayerJoined;
    }

    public class PlayerList : List<IPlayer>, IPlayerList
    {
        public event Action<IPlayer> PlayerJoined = delegate { };

        public new void Add(IPlayer player)
        {
            base.Add(player);

            this.PlayerJoined(player);
        }
    }
}
