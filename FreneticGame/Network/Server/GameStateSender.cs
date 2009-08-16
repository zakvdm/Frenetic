using System;
using Frenetic.Network;
using Frenetic.Player;

namespace Frenetic
{
    public class GameStateSender : IView
    {
        public GameStateSender(Log<ChatMessage> chatLog, IClientStateTracker clientStateTracker, ISnapCounter snapCounter, IOutgoingMessageQueue outgoingMessageQueue)
        {
            this.ChatLog = chatLog;
            this.ClientStateTracker = clientStateTracker;
            this.SnapCounter = snapCounter;
            this.OutgoingMessageQueue = outgoingMessageQueue;
        }


        #region IView Members

        public void Generate()
        {
            if (this.SnapCounter.CurrentSnap > this.LastSentSnap)   // Time to send out state
            {
                this.LastSentSnap = this.SnapCounter.CurrentSnap;

                SendChatLog();

                foreach (Client client in this.ClientStateTracker.NetworkClients)
                {
                    SendPlayerToAllClients(client);
                }

                this.OutgoingMessageQueue.SendMessagesOnQueue();
            }
        }

        #endregion

        void SendChatLog()
        {
            if (this.ChatLog.IsDirty)
            {
                var diffedLog = this.ChatLog.GetDiff();

                this.OutgoingMessageQueue.AddToReliableQueue(new Item() { Type = ItemType.ChatLog, Data = diffedLog });

                this.ChatLog.Clean();
            }
        }

        void SendPlayerToAllClients(Client client)
        {
            IPlayerState state = new PlayerState(client.Player);
            this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = client.ID, Type = ItemType.Player, Data = state });

            if (client.Player.PlayerSettings.IsDirty)
            {
                this.OutgoingMessageQueue.AddToReliableQueue(new Item() { ClientID = client.ID, Type = ItemType.PlayerSettings, Data = client.Player.PlayerSettings.GetDiff() });
                client.Player.PlayerSettings.Clean();
            }
        }

        Log<ChatMessage> ChatLog;
        IClientStateTracker ClientStateTracker;
        IOutgoingMessageQueue OutgoingMessageQueue;
        ISnapCounter SnapCounter;

        int LastSentSnap = 0;
    }
}
