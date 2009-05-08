using System;
using Frenetic.Player;

namespace Frenetic.Network
{
    public class Client
    {
        public delegate Client Factory();

        public Client(IPlayer player, IPlayerSettings playerSettings)
        {
            LastServerSnap = 1; // The server starts counting snaps from 1...
            LastClientSnap = 1; // And so do we...

            Player = player;
            PlayerSettings = playerSettings;
        }

        public int ID { get; set; }
        public IPlayer Player { get; set; }
        public IPlayerSettings PlayerSettings { get; set; }
        public int LastServerSnap { get; set; }
        public int LastClientSnap { get; set; }
    }

    public class LocalClient : Client
    {
        public LocalClient(IPlayer player, LocalPlayerSettings playerSettings)
            : base(player, playerSettings)
        { }
    }
}
