using System;
using Frenetic.Network;

namespace Frenetic
{
    public interface IChatLogDiffer
    {
        bool IsNewClientChatMessage(ChatMessage chatMessage);
        Log<ChatMessage> GetOldestToYoungestDiff(Client client);
    }
}
