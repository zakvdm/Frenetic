using System;

namespace Frenetic.Network
{
    public class Client
    {
        public Client()
        {
            LastServerSnap = 1; // The server starts counting snaps from 1...
        }

        public int ID { get; set; }
        public int LastServerSnap { get; set; }
    }
}
