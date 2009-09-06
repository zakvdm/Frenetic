using System;
using Frenetic.Network;
using Frenetic.Player;
using System.Collections.Generic;

namespace Frenetic
{
    public class GameStateProcessor : IController
    {
        public GameStateProcessor(LocalClient localClient, Log<ChatMessage> chatLog, INetworkPlayerProcessor networkPlayerProcessor, IClientStateTracker clientStateTracker, IIncomingMessageQueue incomingMessageQueue)
        {
            this.LocalClient = localClient;
            this.ChatLog = chatLog;
            this.ClientStateTracker = clientStateTracker;
            this.IncomingMessageQueue = incomingMessageQueue;
            this.NetworkPlayerProcessor = networkPlayerProcessor;
        }

        #region IController Members

        public void Process(float elapsedSeconds)
        {
            // update chat log from server:
            while (this.IncomingMessageQueue.HasAvailable(ItemType.ChatLog))
            {
                var item = this.IncomingMessageQueue.ReadItem(ItemType.ChatLog);

                AddChatMessage(item);
            }
            // update the players:
            while (this.IncomingMessageQueue.HasAvailable(ItemType.Player))
            {
                var item = this.IncomingMessageQueue.ReadItem(ItemType.Player);

                this.NetworkPlayerProcessor.UpdatePlayerFromPlayerStateItem(item);
            }
            // update the player settings:
            while (this.IncomingMessageQueue.HasAvailable(ItemType.PlayerSettings))
            {
                var item = this.IncomingMessageQueue.ReadItem(ItemType.PlayerSettings);

                this.NetworkPlayerProcessor.UpdatePlayerSettingsFromNetworkItem(item);
            }
        }

        #endregion

        void AddChatMessage(Item item)
        {
            var diffedLog = (List<ChatMessage>)item.Data;
            diffedLog.Reverse(); // We receive the List<> with newest element at index 0. We want to iterate from oldest to newest...

            foreach (var msg in diffedLog)
            {
                this.ChatLog.Add(msg);
            }
        }

        LocalClient LocalClient;
        Log<ChatMessage> ChatLog;
        IClientStateTracker ClientStateTracker;
        IIncomingMessageQueue IncomingMessageQueue;
        INetworkPlayerProcessor NetworkPlayerProcessor;
    }
}
