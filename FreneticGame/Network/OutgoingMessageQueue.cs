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
    }
}
