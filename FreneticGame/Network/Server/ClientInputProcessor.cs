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
            while (_incomingMessageQueue.HasAvailable(MessageType.ServerSnap))
            {
                Message message = _incomingMessageQueue.ReadMessage(MessageType.ServerSnap);

                _clientStateTracker.FindNetworkClient(message.ClientID).LastServerSnap = (int)message.Data;
            }
            // Update the last client snap received here by the server:
            while (_incomingMessageQueue.HasAvailable(MessageType.ClientSnap))
            {
                Message message = _incomingMessageQueue.ReadMessage(MessageType.ClientSnap);

                _clientStateTracker.FindNetworkClient(message.ClientID).LastClientSnap = (int)message.Data;
            }
            // Update server chat log:
            while (_incomingMessageQueue.HasAvailable(MessageType.ChatLog))
            {
                Message netMsg = _incomingMessageQueue.ReadMessage(MessageType.ChatLog);

                AddClientChatMessageToServerLog(netMsg);
            }
            // Update player:
            while (_incomingMessageQueue.HasAvailable(MessageType.Player))
            {
                Message netMsg = _incomingMessageQueue.ReadMessage(MessageType.Player);

                _networkPlayerProcessor.UpdatePlayerFromNetworkMessage(netMsg);
            }
            // Update Player Settings:
            while (_incomingMessageQueue.HasAvailable(MessageType.PlayerSettings))
            {
                Message netMsg = _incomingMessageQueue.ReadMessage(MessageType.PlayerSettings);

                _networkPlayerProcessor.UpdatePlayerSettingsFromNetworkMessage(netMsg);
            }
        }

        #endregion

        void AddClientChatMessageToServerLog(Message netMsg)
        {
            ChatMessage chatMsg = (ChatMessage)netMsg.Data;

            chatMsg.ClientName = _clientStateTracker.FindNetworkClient(netMsg.ClientID).PlayerSettings.Name;

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
