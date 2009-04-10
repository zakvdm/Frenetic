using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Frenetic.Network
{
    public interface IOutgoingMessageQueue
    {
        void Write(Message message);
        void Write(Message message, NetChannel channel);
        void WriteFor(Message message, NetChannel channel, int destinationPlayerID);
        void WriteFor(Message message, Client destinationClient);
        void WriteForAllExcept(Message message, NetChannel channel, int excludedPlayerID);
    }
}
