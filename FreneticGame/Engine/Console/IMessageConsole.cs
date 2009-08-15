using System;
using System.Collections.Generic;

namespace Frenetic
{
    public interface IMessageConsole : IConsole<ChatMessage> 
    {
        Log<ChatMessage> PendingLog { get; }
    }
}
