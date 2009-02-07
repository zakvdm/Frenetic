using System;


namespace Frenetic.Network
{
    public interface IIncomingMessageQueue
    {
        object ReadMessage(MessageType type);
    }
}
