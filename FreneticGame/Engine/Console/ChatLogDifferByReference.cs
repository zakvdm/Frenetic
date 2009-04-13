using System;
using Frenetic.Network;

namespace Frenetic
{
    public class ChatLogDifferByReference : IChatLogDiffer
    {
        public ChatLogDifferByReference(Log<ChatMessage> serverChatLog)
        {
            _serverChatLog = serverChatLog;
        }

        #region IChatLogDiffer Members

        public bool IsNewClientChatMessage(ChatMessage chatMessage)
        {
            if (_seenClientMessages.Exists(msg => msg == chatMessage))
                return false;

            // We want to keep a copy in the seen client messages log to compare against later:
            ChatMessage copy = new ChatMessage(chatMessage);
            _seenClientMessages.AddMessage(copy);

            return true;
        }

        public Log<ChatMessage> GetOldestToYoungestDiff(Client client)
        {
            _diffedLog.Clear();

            foreach (ChatMessage chatMsg in _serverChatLog)
            {
                if (chatMsg.Snap <= client.LastServerSnap) // Remember, smaller snap numbers represent older messages
                    break;

                // Note, we're adding the youngest first, so this will read out oldest to youngest...
                _diffedLog.AddMessage(chatMsg);
            }

            if (_diffedLog.Count == 0)
                return null; // We do this so that we don't need to send empty diff logs over the network...

            return _diffedLog;
        }

        #endregion

        Log<ChatMessage> _serverChatLog;

        Log<ChatMessage> _diffedLog = new Log<ChatMessage>();
        Log<ChatMessage> _seenClientMessages = new Log<ChatMessage>();
    }
}
