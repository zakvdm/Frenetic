using System;
using Lidgren.Network;
using Frenetic.Network.Lidgren;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public interface IServerNetworkSession : INetworkSession
    {
        void Create(int port);

        void SendToAll(Message msg, NetChannel channel);
        void SendTo(Message msg, NetChannel channel, int destinationClientID);
        void SendToAllExcept(Message msg, NetChannel channel, int excludedClientID);

        Dictionary<int, INetConnection> ActiveConnections { get; }
    }
}
