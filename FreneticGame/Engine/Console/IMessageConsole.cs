using System;
using System.Collections.Generic;

namespace Frenetic
{
    public interface IMessageConsole : IConsole<ChatMessage> 
    {
        IEnumerable<ChatMessage> UnsortedMessages { get; }

        IEnumerable<ChatMessage> GetPendingMessagesFromAfter(int snap);
    }
}
