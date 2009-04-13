using System;

namespace Frenetic.Network
{
    public class Client
    {
        public Client()
        {
            LastServerSnap = 1; // The server starts counting snaps from 1...
            LastClientSnap = 1; // And so do we...
            Name = "penis";
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int LastServerSnap { get; set; }
        public int LastClientSnap { get; set; }
    }
}
