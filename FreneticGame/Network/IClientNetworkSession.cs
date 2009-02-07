using System;
using Lidgren.Network;

namespace Frenetic.Network
{
    public interface IClientNetworkSession : INetworkSession
    {
        void Join(string IP, int port);
        void Join(int port);

        void SendToServer(Message msg, NetChannel channel);
    }
}
