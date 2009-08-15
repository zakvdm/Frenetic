using System;
using Frenetic.Network;
using Frenetic.Player;

namespace Frenetic
{
    public class GameStateSender : IView
    {
        public GameStateSender(IChatLogDiffer chatLogDiffer, IClientStateTracker clientStateTracker, ISnapCounter snapCounter, IOutgoingMessageQueue outgoingMessageQueue)
        {
            this.ChatLogDiffer = chatLogDiffer;
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

                foreach (Client client in this.ClientStateTracker.NetworkClients)
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
            this.OutgoingMessageQueue.WriteFor(new Message() { Items = { new Item() { Type = ItemType.ServerSnap, Data = this.LastSentSnap } } }, client);

            // Send the last received client snap:
            this.OutgoingMessageQueue.WriteFor(new Message() { Items = { new Item() { Type = ItemType.ClientSnap, Data = client.LastClientSnap } } }, client);
        }

        void SendChatLog(Client client)
        {
            Log<ChatMessage> diffedLog = this.ChatLogDiffer.GetOldestToYoungestDiff(client);

            if (diffedLog == null) // Diff didn't return new messages
                return;

            // We send the messages oldest to newest...
            foreach (var message in diffedLog)
            {
                Message msg = new Message() { Items = { new Item() { Type = ItemType.ChatLog, Data = message } } };

                // Send chat msg to clients
                this.OutgoingMessageQueue.WriteFor(msg, client);
            }
        }

        void SendPlayerToAllClients(Client client)
        {
            IPlayerState state = new PlayerState(client.Player);
            //this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = client.ID, Type = ItemType.Player, Data = state });
            //this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = client.ID, Type = ItemType.PlayerSettings, Data = client.Player.PlayerSettings });

            this.OutgoingMessageQueue.Write(new Message() { Items = { new Item() { ClientID = client.ID, Type = ItemType.Player, Data = state }}});
            this.OutgoingMessageQueue.Write(new Message() { Items = { new Item() { ClientID = client.ID, Type = ItemType.PlayerSettings, Data = client.Player.PlayerSettings }}});
        }

        IChatLogDiffer ChatLogDiffer;
        IClientStateTracker ClientStateTracker;
        IOutgoingMessageQueue OutgoingMessageQueue;
        ISnapCounter SnapCounter;

        int LastSentSnap = 0;
    }
}
