using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Frenetic.Network
{
    public interface IOutgoingMessageQueue
    {
        void AddToQueue(Item item);
        void AddToReliableQueue(Item item);

        void SendMessagesOnQueue();
    }
}
