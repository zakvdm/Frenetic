using System;

namespace Frenetic
{
    public class MessageConsole : IMessageConsole
    {
        public MessageConsole(MessageLog chatLog, MessageLog newMessages)
        {
            Log = chatLog;
            _newMessages = newMessages;
        }

        #region IConsole Members

        public bool Active { get; set; }
        
        public void ProcessInput(string input)
        {
            _newMessages.AddMessage(input);
        }

        #endregion

        public bool HasNewMessages
        {
            get
            {
                return _newMessages.Count > 0;
            }
        }
        public string GetNewMessage()
        {
            // We want to return the oldest new message first:
            return _newMessages.StripOldestMessage();

        }

        public MessageLog Log { get; set; }
        MessageLog _newMessages;

    }
}
