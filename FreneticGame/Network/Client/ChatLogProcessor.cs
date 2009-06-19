﻿using System;
using Frenetic.Network;
using Frenetic.Player;

namespace Frenetic
{
    public class ChatLogProcessor : IController
    {
        public ChatLogProcessor(LocalClient localClient, Log<ChatMessage> chatLog, INetworkPlayerProcessor networkPlayerProcessor, IClientStateTracker clientStateTracker, IIncomingMessageQueue incomingMessageQueue)
        {
            _localClient = localClient;
            _chatLog = chatLog;
            _clientStateTracker = clientStateTracker;
            _incomingMessageQueue = incomingMessageQueue;
            _networkPlayerProcessor = networkPlayerProcessor;
        }

        #region IController Members

        public void Process(float elapsedTime)
        {
            // Set most recently received server snap:
            while (_incomingMessageQueue.HasAvailable(MessageType.ServerSnap))
            {
                Message message = _incomingMessageQueue.ReadMessage(MessageType.ServerSnap);
                _localClient.LastServerSnap = (int)message.Data;
            }

            // Set last acknowledged client snap:
            while (_incomingMessageQueue.HasAvailable(MessageType.ClientSnap))
            {
                Message message = _incomingMessageQueue.ReadMessage(MessageType.ClientSnap);
                _localClient.LastClientSnap = (int)message.Data;
            }

            // update chat log from server:
            while (_incomingMessageQueue.HasAvailable(MessageType.ChatLog))
            {
                Message message = _incomingMessageQueue.ReadMessage(MessageType.ChatLog);

                AddChatMessage((ChatMessage)message.Data);
            }
            // update the players:
            while (_incomingMessageQueue.HasAvailable(MessageType.Player))
            {
                Message netMsg = _incomingMessageQueue.ReadMessage(MessageType.Player);

                _networkPlayerProcessor.UpdatePlayerFromNetworkMessage(netMsg);
            }
            // update the player settings:
            while (_incomingMessageQueue.HasAvailable(MessageType.PlayerSettings))
            {
                Message netMsg = _incomingMessageQueue.ReadMessage(MessageType.PlayerSettings);

                _networkPlayerProcessor.UpdatePlayerSettingsFromNetworkMessage(netMsg);
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
