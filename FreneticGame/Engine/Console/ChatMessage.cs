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
            Snap = copySource.Snap;
            Message = copySource.Message;
        }

        public override string ToString()
        {
            return ("[" + ClientName + "] " + Message);
        }

        public static bool operator ==(ChatMessage lhs, ChatMessage rhs)
        {
            return ((lhs.ClientName == rhs.ClientName) && (lhs.Snap == rhs.Snap) && (lhs.Message == rhs.Message));
        }
        public static bool operator !=(ChatMessage lhs, ChatMessage rhs)
        {
            return ((lhs.ClientName != rhs.ClientName) || (lhs.Snap != rhs.Snap) || (lhs.Message != rhs.Message));
        }

        #region Other overrides
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        public string ClientName { get; set; }
        public int Snap { get; set; }
        public string Message { get; set; }
    }
}
