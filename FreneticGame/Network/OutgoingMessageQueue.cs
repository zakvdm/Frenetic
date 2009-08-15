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

        public void AddToQueue(Item item)
        {
            this.CurrentMessage.Items.Add(item);
        }
        public void AddToReliableQueue(Item item)
        {
            this.NetworkSession.Send(new Message() { Items = { item } }, NetChannel.ReliableUnordered);
        }

        public void SendMessagesOnQueue()
        {
            this.NetworkSession.Send(this.CurrentMessage, NetChannel.UnreliableInOrder1);

            this.CurrentMessage = new Message();
        }

        public Message CurrentMessage { get; private set;  }

        INetworkSession NetworkSession;

        // TODO: Delete everything below here (once i dont' need it anymore...)
        
        private void Write(Message message)
        {
            Write(message, NetChannel.UnreliableInOrder1);
        }

        // TODO: Make private
        private void Write(Message message, NetChannel channel)
        {
            /*
            if (_clientNetworkSession != null)
            {
                _clientNetworkSession.Send(message, channel);
            }
            if (_serverNetworkSession != null)
            {
                _serverNetworkSession.SendToAll(message, channel);
            }*/
        }

        private void WriteFor(Message message, Client destinationClient)
        {
            WriteFor(message, NetChannel.Unreliable, destinationClient.ID);
        }

        // TODO: Make private...
        private void WriteFor(Message message, NetChannel channel, int destinationPlayerID)
        {
            /*
            _serverNetworkSession.SendTo(message, channel, destinationPlayerID);
             */
        }
    }
}
