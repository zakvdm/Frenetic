using System;
using Frenetic.Player;

namespace Frenetic.Network
{
    public class ClientInputProcessor : IController
    {
        public ClientInputProcessor(INetworkPlayerProcessor networkPlayerProcessor, Log<ChatMessage> serverChatLog, IClientStateTracker clientStateTracker, IChatLogDiffer chatLogDiffer, ISnapCounter snapCounter, IIncomingMessageQueue incomingMessageQueue)
        {
            _networkPlayerProcessor = networkPlayerProcessor;
            _serverChatLog = serverChatLog;
            _clientStateTracker = clientStateTracker;
            _chatLogDiffer = chatLogDiffer;
            _snapCounter = snapCounter;
            _incomingMessageQueue = incomingMessageQueue;
        }

        #region IController Members

        public void Process(float elapsedTime)
        {
            // Update the last server snap received by the client:
            while (_incomingMessageQueue.HasAvailable(ItemType.ServerSnap))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.ServerSnap);

                _clientStateTracker.FindNetworkClient(item.ClientID).LastServerSnap = (int)item.Data;
            }
            // Update the last client snap received here by the server:
            while (_incomingMessageQueue.HasAvailable(ItemType.ClientSnap))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.ClientSnap);

                _clientStateTracker.FindNetworkClient(item.ClientID).LastClientSnap = (int)item.Data;
            }
            // Update server chat log:
            while (_incomingMessageQueue.HasAvailable(ItemType.ChatLog))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.ChatLog);

                AddClientChatMessageToServerLog(item);
            }
            // Update player:
            while (_incomingMessageQueue.HasAvailable(ItemType.Player))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.Player);

                _networkPlayerProcessor.UpdatePlayerFromNetworkItem(item);
            }
            // Update Player Settings:
            while (_incomingMessageQueue.HasAvailable(ItemType.PlayerSettings))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.PlayerSettings);

                _networkPlayerProcessor.UpdatePlayerSettingsFromNetworkItem(item);
            }
        }

        #endregion

        void AddClientChatMessageToServerLog(Item item)
        {
            ChatMessage chatMsg = (ChatMessage)item.Data;

            chatMsg.ClientName = _clientStateTracker.FindNetworkClient(item.ClientID).Player.PlayerSettings.Name;

            // Before we add this message to the server log, let's check that we haven't already added it
            if (_chatLogDiffer.IsNewClientChatMessage(chatMsg))
            {
                // ChatMessages should be added to the server log with the current server snap:
                chatMsg.Snap = _snapCounter.CurrentSnap;

                _serverChatLog.AddMessage(chatMsg);
            }
        }

        INetworkPlayerProcessor _networkPlayerProcessor;
        Log<ChatMessage> _serverChatLog;
        IClientStateTracker _clientStateTracker;
        IChatLogDiffer _chatLogDiffer;
        ISnapCounter _snapCounter;
        IIncomingMessageQueue _incomingMessageQueue;
    }
}
