using System;
using System.Collections.Generic;

namespace Frenetic
{
    public class MessageConsole : IMessageConsole
    {
        public MessageConsole(Log<ChatMessage> chatLog, Log<ChatMessage> newMessages)
        {
            Log = chatLog;
            _pendingMessages = newMessages;
        }

        #region IConsole Members

        public bool Active { get; set; }
        
        public void ProcessInput(string input)
        {
            // We just bundle all new messages in UnsortedMessages until someone comes along later and sets their Snap property...
            ChatMessage msg = new ChatMessage() { Snap = 0, Message = input };
            
            _pendingMessages.AddMessage(msg);
        }

        #endregion

        #region IMessageConsole Members

        public IEnumerable<ChatMessage> UnsortedMessages
        {
            get
            {
                foreach (ChatMessage msg in _pendingMessages)
                {
                    if (msg.Snap == 0) // ChatMessages with Snap set to 0 have not been "sorted" yet...
                    {
                        yield return msg;
                    }
                }
            }
        }

        public IEnumerable<ChatMessage> GetPendingMessagesFromAfter(int snap)
        {
            foreach (ChatMessage msg in _pendingMessages)
            {
                if (msg.Snap <= snap)
                {
                    // We only want messages that are younger than this point -- remember, the lower the snap, the older the message
                    break;
                }
                yield return msg;
            }
        }
        
        #endregion
        public Log<ChatMessage> Log { get; set; }
        Log<ChatMessage> _pendingMessages;
    }
}
