using System;

namespace Frenetic.Network
{
    public interface IIncomingMessageQueue
    {
        Message ReadWholeMessage(MessageType type);

        // TODO: Remove this...
        object ReadMessage(MessageType type);
    }
}
