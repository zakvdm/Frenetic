using System;
using System.Collections.Generic;

namespace Frenetic
{
    public class MessageConsole : IMessageConsole
    {
        public MessageConsole(Log<ChatMessage> chatLog, Log<ChatMessage> newMessages)
        {
            Log = chatLog;
            this.PendingLog = newMessages;
        }

        public bool Active { get; set; }
        
        public void ProcessInput(string input)
        {
            this.PendingLog.Add(new ChatMessage() { Message = input });
        }

        public Log<ChatMessage> Log { get; set; }
        public Log<ChatMessage> PendingLog { get; private set; }
    }
}
