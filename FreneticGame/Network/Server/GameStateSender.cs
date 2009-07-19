using System;
using Frenetic.Network;
using Frenetic.Player;

namespace Frenetic
{
    public class GameStateSender : IView
    {
        public GameStateSender(IChatLogDiffer chatLogDiffer, IClientStateTracker clientStateTracker, ISnapCounter snapCounter, IOutgoingMessageQueue outgoingMessageQueue)
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

                foreach (Client client in _clientStateTracker.NetworkClients)
                {
                    SendServerAndClientSnap(client); // We always send the latest snap

                    SendChatLog(client);

                    SendPlayerToAllClients(client);
                }
            }
        }

        #endregion

        void SendServerAndClientSnap(Client client)
        {
            // Send the current server snap:
            _outgoingMessageQueue.WriteFor(new Message() { Type = MessageType.ServerSnap, Data = _lastSentSnap }, client);

            // Send the last received client snap:
            _outgoingMessageQueue.WriteFor(new Message() { Type = MessageType.ClientSnap, Data = client.LastClientSnap }, client);
        }

        void SendChatLog(Client client)
        {
            Log<ChatMessage> diffedLog = _chatLogDiffer.GetOldestToYoungestDiff(client);

            if (diffedLog == null) // Diff didn't return new messages
                return;

            // We send the messages oldest to newest...
            foreach (var message in diffedLog)
            {
                Message msg = new Message() { Type = MessageType.ChatLog, Data = message };

                // Send chat msg to clients
                _outgoingMessageQueue.WriteFor(msg, client);
            }
        }

        void SendPlayerToAllClients(Client client)
        {
            IPlayerState state = new PlayerState(client.Player);
            _outgoingMessageQueue.Write(new Message() { ClientID = client.ID, Type = MessageType.Player, Data = state });

            _outgoingMessageQueue.Write(new Message() { ClientID = client.ID, Type = MessageType.PlayerSettings, Data = client.Player.PlayerSettings });
        }

        IChatLogDiffer _chatLogDiffer;
        IClientStateTracker _clientStateTracker;
        IOutgoingMessageQueue _outgoingMessageQueue;
        ISnapCounter _snapCounter;

        int _lastSentSnap = 0;
    }
}
