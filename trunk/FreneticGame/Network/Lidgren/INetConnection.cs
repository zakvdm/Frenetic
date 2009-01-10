using System;
using System.Net;
using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    public interface INetConnection
    {
        NetConnection NetConnection { get; }
        IPEndPoint RemoteEndPoint { get; }
        NetConnectionStatus Status { get; }

        int ConnectionID { get; }
        
        void Approve();
    }
}
