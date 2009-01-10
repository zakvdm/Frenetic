using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    public class LidgrenNetworkSessionFactory : INetworkSessionFactory
    {
        public INetworkSession MakeServerNetworkSession()
        {
            // create a configuration for the server
            // NOTE: "Frenetic" string is IMPORTANT (must be the same on client AND server config)
            NetConfiguration config = new NetConfiguration("Frenetic");
            config.MaxConnections = 128;
            config.Port = 14242;

            NetServerWrapper server = new NetServerWrapper(new NetServer(config));

            LidgrenNetworkSession serverNS = new LidgrenNetworkSession(server);
            
            serverNS.Create();  // TODO: This is not right...

            return serverNS;
        }

        public INetworkSession MakeClientNetworkSession()
        {
            NetConfiguration config = new NetConfiguration("Frenetic");
            NetClientWrapper client = new NetClientWrapper(new NetClient(config));

            LidgrenNetworkSession clientNS = new LidgrenNetworkSession(client);

            clientNS.Join(14242);   // TODO: This is not right... (magic number too)

            return clientNS;
        }
    }
}
