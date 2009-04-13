using System;
using Frenetic.Network;

namespace Frenetic
{
    public class ChatLogSender : IView
    {
        public ChatLogSender(IChatLogDiffer chatLogDiffer, IClientStateTracker clientStateTracker, ISnapCounter snapCounter, IOutgoingMessageQueue outgoingMessageQueue)
        {
            _chatLogDiffer = chatLogDiffer;
            _clientStateTracker = clientStateTracker;
            _snapCounter = snapCounter;
            _outgoingMessageQueue = outgoingMessageQueue;
        }


        #region IView Members

        public void Generate()
        {
            if (_snapCounter.CurrentSnap > _lastSentSnap)   // Time to send out state
            {
                _lastSentSnap = _snapCounter.CurrentSnap;

                foreach (Client client in _clientStateTracker.CurrentClients)
                {
                    SendServerAndClientSnap(client); // We always send the latest snap

                    Log<ChatMessage> diffedLog = _chatLogDiffer.GetOldestToYoungestDiff(client);
                    if (diffedLog != null) // Diff returned new messages
                        SendLog(diffedLog, client);
                }
            }
        }

        #endregion

        void SendLog(Log<ChatMessage> log, Client client)
        {
            // We send the messages oldest to newest...
            //foreach (var message in log.OldestToNewest)
            foreach (var message in log)
            {
                Message msg = new Message() { Type = MessageType.ChatLog, Data = message };

                // Send chat msg to clients
                _outgoingMessageQueue.WriteFor(msg, client);
            }
        }
        void SendServerAndClientSnap(Client client)
        {
            // Send the current server snap:
            _outgoingMessageQueue.WriteFor(new Message() { Type = MessageType.ServerSnap, Data = _lastSentSnap }, client);

            // Send the last received client snap:
            _outgoingMessageQueue.WriteFor(new Message() { Type = MessageType.ClientSnap, Data = client.LastClientSnap }, client);
        }

        IChatLogDiffer _chatLogDiffer;
        IClientStateTracker _clientStateTracker;
        IOutgoingMessageQueue _outgoingMessageQueue;
        ISnapCounter _snapCounter;

        int _lastSentSnap = 0;
    }
}
