using System;

namespace Frenetic.Network
{
    public interface IIncomingMessageQueue
    {
        bool HasAvailable(ItemType type);

        Item ReadItem(ItemType type);
    }
}
