using System;
using Frenetic.Player;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public class ClientInputProcessor : IController
    {
        public ClientInputProcessor(INetworkPlayerProcessor networkPlayerProcessor, Log<ChatMessage> serverChatLog, IClientStateTracker clientStateTracker, IIncomingMessageQueue incomingMessageQueue)
        {
            this.NetworkPlayerProcessor = networkPlayerProcessor;
            this.ServerChatLog = serverChatLog;
            this.ClientStateTracker = clientStateTracker;
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
        }

        INetworkPlayerProcessor NetworkPlayerProcessor;
        Log<ChatMessage> ServerChatLog;
        IClientStateTracker ClientStateTracker;
        IIncomingMessageQueue IncomingMessageQueue;
    }
}
