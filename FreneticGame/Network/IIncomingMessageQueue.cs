using System;

namespace Frenetic.Network
{
    public interface IIncomingMessageQueue
    {
        bool HasAvailable(MessageType type);

        Message ReadWholeMessage(MessageType type);
    }
}
