using System;
using Lidgren.Network;

namespace Frenetic.Network
{
    public class OutgoingMessageQueue : IOutgoingMessageQueue
    {
        public OutgoingMessageQueue(INetworkSession networkSession)
        {
            this.NetworkSession = networkSession;

            this.CurrentMessage = new Message();
        }
        public OutgoingMessageQueue(INetworkSession networkSession, int TODOtemp)
            : this(networkSession)
        { }

        public void AddToQueue(Item item)
        {
            this.CurrentMessage.Items.Add(item);
        }

        public void SendMessagesOnQueue()
        {
            this.NetworkSession.Send(this.CurrentMessage, NetChannel.Unreliable);

            this.CurrentMessage = new Message();
        }

        public Message CurrentMessage { get; private set;  }

        INetworkSession NetworkSession;

        // TODO: Delete everything below here (once i dont' need it anymore...)
        public OutgoingMessageQueue(IClientNetworkSession clientNetworkSession, IServerNetworkSession serverNetworkSession)
        {
            _clientNetworkSession = clientNetworkSession;
            _serverNetworkSession = serverNetworkSession;
        }
        
        public void Write(Message message)
        {
            Write(message, NetChannel.UnreliableInOrder1);
        }

        // TODO: Make private
        public void Write(Message message, NetChannel channel)
        {
            if (_clientNetworkSession != null)
            {
                _clientNetworkSession.Send(message, channel);
            }
            if (_serverNetworkSession != null)
            {
                _serverNetworkSession.SendToAll(message, channel);
            }
        }

        public void WriteFor(Message message, Client destinationClient)
        {
            WriteFor(message, NetChannel.Unreliable, destinationClient.ID);
        }

        // TODO: Make private...
        public void WriteFor(Message message, NetChannel channel, int destinationPlayerID)
        {
            _serverNetworkSession.SendTo(message, channel, destinationPlayerID);
        }

        
        IClientNetworkSession _clientNetworkSession;
        IServerNetworkSession _serverNetworkSession;
    }
}
