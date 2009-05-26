using Lidgren.Network;
using Frenetic.Network.Lidgren;

namespace Frenetic.Network
{
    public interface IServerMessageSender
    {
        void SendTo(Message msg, NetChannel channel, INetConnection destinationConnection);
        void SendToAll(Message msg, NetChannel channel);
        void SendToAllExcept(Message msg, NetChannel channel, INetConnection excludedConnection);
    }
}
