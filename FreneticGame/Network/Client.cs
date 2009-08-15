using System;
using Frenetic.Player;

namespace Frenetic.Network
{
    public class Client
    {
        public delegate Client Factory();

        public Client(IPlayer player)
        {
            Player = player;
        }

        public int ID { get; set; }
        public IPlayer Player { get; set; }
    }

    public class LocalClient : Client
    {
        public LocalClient(IPlayer player)
            : base(player)
        { }
    }
}
