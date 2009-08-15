using System;
using Frenetic.Network;
using Frenetic.Player;

namespace Frenetic
{
    public class GameStateProcessor : IController
    {
        public GameStateProcessor(LocalClient localClient, Log<ChatMessage> chatLog, INetworkPlayerProcessor networkPlayerProcessor, IClientStateTracker clientStateTracker, IIncomingMessageQueue incomingMessageQueue)
        {
            _localClient = localClient;
            _chatLog = chatLog;
            _clientStateTracker = clientStateTracker;
            _incomingMessageQueue = incomingMessageQueue;
            _networkPlayerProcessor = networkPlayerProcessor;
        }

        #region IController Members

        public void Process(float elapsedSeconds)
        {
            // Set most recently received server snap:
            while (_incomingMessageQueue.HasAvailable(ItemType.ServerSnap))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.ServerSnap);
                _localClient.LastServerSnap = (int)item.Data;
            }

            // Set last acknowledged client snap:
            while (_incomingMessageQueue.HasAvailable(ItemType.ClientSnap))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.ClientSnap);
                _localClient.LastClientSnap = (int)item.Data;
            }

            // update chat log from server:
            while (_incomingMessageQueue.HasAvailable(ItemType.ChatLog))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.ChatLog);

                AddChatMessage((ChatMessage)item.Data);
            }
            // update the players:
            while (_incomingMessageQueue.HasAvailable(ItemType.Player))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.Player);

                _networkPlayerProcessor.UpdatePlayerFromPlayerStateItem(item);
            }
            // update the player settings:
            while (_incomingMessageQueue.HasAvailable(ItemType.PlayerSettings))
            {
                var item = _incomingMessageQueue.ReadItem(ItemType.PlayerSettings);

                _networkPlayerProcessor.UpdatePlayerSettingsFromNetworkItem(item);
            }
        }

        #endregion

        void AddChatMessage(ChatMessage newMsg)
        {
            // We don't want to add duplicate messages:
            if (_chatLog.Exists(msg => msg == newMsg))
                return;

            _chatLog.AddMessage(newMsg);
        }

        LocalClient _localClient;
        Log<ChatMessage> _chatLog;
        IClientStateTracker _clientStateTracker;
        IIncomingMessageQueue _incomingMessageQueue;
        INetworkPlayerProcessor _networkPlayerProcessor;
    }
}
