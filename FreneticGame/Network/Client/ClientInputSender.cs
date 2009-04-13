using System;
using Lidgren.Network;

namespace Frenetic.Network
{
    public class ClientInputSender : IView
    {
        public ClientInputSender(Client localClient, IMessageConsole messageConsole, ISnapCounter snapCounter, IOutgoingMessageQueue outgoingMessageQueue)
        {
            _localClient = localClient;
            _messageConsole = messageConsole;
            _snapCounter = snapCounter;
            _outgoingMessageQueue = outgoingMessageQueue;
        }

        #region IView Members

        public void Generate()
        {
            // If the client ID is still 0 then we can't be connected yet, so no point in trying to send...
            if (_localClient.ID == 0)
                return;

            if (_snapCounter.CurrentSnap > _lastSentSnap)
            {
                _lastSentSnap = _snapCounter.CurrentSnap;

                SendLastReceivedServerSnapAndCurrentClientSnap();

                SendAllPendingChatMessages();
            }
        }

        #endregion

        void SendLastReceivedServerSnapAndCurrentClientSnap()
        {
            // the last received server snap:
            _outgoingMessageQueue.Write(new Message() { ClientID = _localClient.ID, Type = MessageType.ServerSnap, Data = _localClient.LastServerSnap });

            // the current client snap:
            _outgoingMessageQueue.Write(new Message() { ClientID = _localClient.ID, Type = MessageType.ClientSnap, Data = _snapCounter.CurrentSnap });
        }

        void SendAllPendingChatMessages()
        {
            // First. If there are new messages, we need to set the current client snap on them so we can keep sending them until this snap is acknowledged.
            foreach (ChatMessage newMsg in _messageConsole.UnsortedMessages)
            {
                newMsg.Snap = _snapCounter.CurrentSnap;
            }

            // NOTE: We use the LastClientSnap on the local client as the last client snap acknowledged by the server
            foreach (ChatMessage unAckedMsg in _messageConsole.GetPendingMessagesFromAfter(_localClient.LastClientSnap))
            {
                _outgoingMessageQueue.Write(new Message() { ClientID = _localClient.ID, Type = MessageType.ChatLog, Data = unAckedMsg });
            }
        }

        IMessageConsole _messageConsole;
        IOutgoingMessageQueue _outgoingMessageQueue;
        Client _localClient;
        ISnapCounter _snapCounter;

        int _lastSentSnap = 0;
    }
}
