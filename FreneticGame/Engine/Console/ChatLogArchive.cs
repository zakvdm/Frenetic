using System;
using System.Collections.Generic;
using Frenetic.Network;

namespace Frenetic
{
    public class ChatLogArchive : IChatLogArchive
    {
        // NOTE: Expect this implementation to be very inefficient on memory use (since a new copy of the chat log is stored every snap).
        //          Could be improved by keeping a single copy and indexing it somehow...
        public ChatLogArchive(MessageLog serverChatLog, ISnapCounter snapCounter)
        {
            _serverChatLog = serverChatLog;
            _snapCounter = snapCounter;
        }

        public MessageLog this[Client client]
        {
            get
            {
                if (!_logArchive.ContainsKey(client.LastServerSnap))
                    return null;

                return _logArchive[client.LastServerSnap];
            }
        }

        #region IController Members

        public void Process(float elapsedTime)
        {
            if (_snapCounter.CurrentSnap > _previousSnap)
            {
                while (_previousSnap < _snapCounter.CurrentSnap)   // Do this loop in case we need to catch up more than 1 snap (should usually only loop once)
                {
                    _previousSnap++;
                    _logArchive.Add(_previousSnap, _serverChatLog.Copy());
                }
            }
        }

        #endregion

        MessageLog _serverChatLog;
        ISnapCounter _snapCounter;
        int _previousSnap = 0;
        Dictionary<int, MessageLog> _logArchive = new Dictionary<int, MessageLog>();
    }
}
