using System;
using Frenetic.Player;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public class ClientInputProcessor : IController
    {
        public ClientInputProcessor(INetworkPlayerProcessor networkPlayerProcessor, Log<ChatMessage> serverChatLog, IClientStateTracker clientStateTracker, ISnapCounter snapCounter, IIncomingMessageQueue incomingMessageQueue)
        {
            this.NetworkPlayerProcessor = networkPlayerProcessor;
            this.ServerChatLog = serverChatLog;
            this.ClientStateTracker = clientStateTracker;
            this.SnapCounter = snapCounter;
            this.IncomingMessageQueue = incomingMessageQueue;
        }

        #region IController Members

        public void Process(float elapsedTime)
        {
            // Update server chat log:
            while (this.IncomingMessageQueue.HasAvailable(ItemType.ChatLog))
            {
                var item = this.IncomingMessageQueue.ReadItem(ItemType.ChatLog);

                AddClientChatMessagesToServerLog(item);
            }
            // Update player:
            while (this.IncomingMessageQueue.HasAvailable(ItemType.Player))
            {
                var item = this.IncomingMessageQueue.ReadItem(ItemType.Player);

                this.NetworkPlayerProcessor.UpdatePlayerFromNetworkItem(item);
            }
            // Update Player Settings:
            while (this.IncomingMessageQueue.HasAvailable(ItemType.PlayerSettings))
            {
                var item = this.IncomingMessageQueue.ReadItem(ItemType.PlayerSettings);

                this.NetworkPlayerProcessor.UpdatePlayerSettingsFromNetworkItem(item);
            }
        }

        #endregion

        void AddClientChatMessagesToServerLog(Item item)
        {
            var diffedLog = (List<ChatMessage>)item.Data;

            foreach (var chatMsg in diffedLog)
            {
                chatMsg.ClientName = this.ClientStateTracker.FindNetworkClient(item.ClientID).Player.PlayerSettings.Name;
                this.ServerChatLog.AddMessage(chatMsg);
            }


            /*
             * TODO: DELETE
            ChatMessage chatMsg = (ChatMessage)item.Data;

            chatMsg.ClientName = this.ClientStateTracker.FindNetworkClient(item.ClientID).Player.PlayerSettings.Name;

            // Before we add this message to the server log, let's check that we haven't already added it
            if (_chatLogDiffer.IsNewClientChatMessage(chatMsg))
            {
                // ChatMessages should be added to the server log with the current server snap:
                chatMsg.Snap = this.SnapCounter.CurrentSnap;

                this.ServerChatLog.AddMessage(chatMsg);
            }
             */
        }

        INetworkPlayerProcessor NetworkPlayerProcessor;
        Log<ChatMessage> ServerChatLog;
        IClientStateTracker ClientStateTracker;
        ISnapCounter SnapCounter; // TODO: DO I STILL NEED THIS?
        IIncomingMessageQueue IncomingMessageQueue;
    }
}
