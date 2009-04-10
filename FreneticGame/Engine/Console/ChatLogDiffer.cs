using System;
using Frenetic.Network;

namespace Frenetic
{
    public class ChatLogDiffer : IChatLogDiffer
    {
        public ChatLogDiffer(MessageLog serverChatLog, IChatLogArchive chatLogArchive)
        {
            _currentServerChatLog = serverChatLog;
            _chatLogArchive = chatLogArchive;
        }

        public MessageLog Diff(Client client)
        {
            if (_currentServerChatLog.Count == 0)
                return null;    // Can't be anything new since there's nothing yet in the chat log...

            MessageLog archivedLog = _chatLogArchive[client];

            if (archivedLog == null) // Not much more we can do here
                return null;

            return FindDifferenceBetweenOldAndNewLog(archivedLog, _currentServerChatLog);
        }

        MessageLog FindDifferenceBetweenOldAndNewLog(MessageLog oldLog, MessageLog newLog)
        {
            // We need to add the messages from oldest to newest, so first we count up and then we count down again... (index 0 is the newest message)
            int count = 0;

            if (oldLog.Count == 0)  // If the archived log is empty, then we need to return *everything* in the new log
            {
                count = newLog.Count;
            }
            else // otherwise, we look for the point where the two logs stop being different
            {
                while (newLog[count] != oldLog[0])
                {
                    count++;    // Count up...
                }
            }

            MessageLog diffedLog;
            if (count == 0)
            {
                // Nothing new
                return null;
            }
            else
            {
                // Get ready to store the differences
                diffedLog = new MessageLog();
            }

            while (count > 0)
            {
                diffedLog.AddMessage(newLog[count - 1]); // Subtract 1 since indexes are zero-based
                count--; // ...and count down again
            }

            return diffedLog;

        }

        MessageLog _currentServerChatLog;
        IChatLogArchive _chatLogArchive;
    }
}
