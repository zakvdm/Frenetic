using System;

using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    class LidgrenNetworkFactory
    {
        public static LidgrenNetworkSession GetServerNetworkSession()
        {
            NetConfiguration config = new NetConfiguration("NetServer");
            config.MaxConnections = 8;
            config.Port = 12345;

            return new LidgrenNetworkSession(new NetServerWrapper(new NetServer(config)));
        }

        public static LidgrenNetworkSession GetClientNetworkSession()
        {
            throw new System.NotImplementedException();
        }
    }
}
