using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Player
{
    public interface IPlayerList : IEnumerable<IPlayer>
    {
        void Add(IPlayer player);

        List<IPlayer> Players { get; }
        event Action<IPlayer> PlayerAdded;
    }

    public class PlayerList : IPlayerList
    {
        public PlayerList()
        {
            this.Players = new List<IPlayer>();
        }

        public List<IPlayer> Players { get; set; }
        public event Action<IPlayer> PlayerAdded = delegate { };

        public void Add(IPlayer player)
        {
            this.Players.Add(player);

            this.PlayerAdded(player);
        }

        public IEnumerator<IPlayer> GetEnumerator()
        {
            foreach (IPlayer player in this.Players)
                yield return player;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
