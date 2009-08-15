using System;

namespace Frenetic
{
    public class ChatMessage
    {
        public ChatMessage()
        { }
        public ChatMessage(ChatMessage copySource)
        {
            ClientName = copySource.ClientName;
            Message = copySource.Message;
        }

        public override string ToString()
        {
            return ("[" + ClientName + "] " + Message);
        }

        public string ClientName { get; set; }
        public string Message { get; set; }
    }
}
