using System;
using System.Net;

using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    public interface INetClient
    {
        NetConnectionStatus Status { get; }

        void Start();
        void Connect(string IP, int port);
        void Connect(IPEndPoint IPEndPoint, byte[] hail);
        void Shutdown(string reason);

        void SendMessage(NetBuffer data, NetChannel channel);
        bool ReadMessage(NetBuffer intoBuffer, out NetMessageType type);
        NetBuffer CreateBuffer();
        NetBuffer CreateBuffer(int initialCapacity);

        void DiscoverLocalServers(int port);
    }
}
