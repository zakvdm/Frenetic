using System;

using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    public interface INetServer
    {
        int Port { get; set; }
        bool Connected { get; }
        bool IsListening { get; }

        void Start();
        void Shutdown(string reason);

        void SendToAll(NetBuffer data, NetChannel channel);
        void SendToAll(NetBuffer data, NetChannel channel, INetConnection exclude);
        void SendMessage(NetBuffer data, NetChannel channel, INetConnection connection);

        bool ReadMessage(NetBuffer intoBuffer, out NetMessageType type, out INetConnection sender);
        NetBuffer CreateBuffer();
        NetBuffer CreateBuffer(int initialCapacity);
    }
}
