using System;
using Frenetic.Player;

namespace Frenetic.Network
{
    public class Client
    {
        // TODO: Remove reference to PlayerSettings (now held by actual IPlayer...)

        public delegate Client Factory();

        public Client(IPlayer player)
        {
            LastServerSnap = 1; // The server starts counting snaps from 1...
            LastClientSnap = 1; // And so do we...

            Player = player;
        }

        public int ID { get; set; }
        public IPlayer Player { get; set; }
        public int LastServerSnap { get; set; }
        public int LastClientSnap { get; set; }
    }

    public class LocalClient : Client
    {
        public LocalClient(IPlayer player)
            : base(player)
        { }
    }
}
