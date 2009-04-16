using System;

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
            while (true)
            {
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ServerSnap);

                if (message == null)
                    break;

                _clientStateTracker[message.ClientID].LastServerSnap = (int)message.Data;
            }
            // Update the last client snap received here by the server:
            while (true)
            {
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ClientSnap);

                if (message == null)
                    break;

                _clientStateTracker[message.ClientID].LastClientSnap = (int)message.Data;
            }
            // Update server chat log:
            while (true)
            {
                Message netMsg = _incomingMessageQueue.ReadWholeMessage(MessageType.ChatLog);

                if (netMsg == null)
                    break;

                AddClientChatMessageToServerLog(netMsg);
            }
            // Update player:
            while (true)
            {
                Message netMsg = _incomingMessageQueue.ReadWholeMessage(MessageType.Player);

                if (netMsg == null)
                    break;

                _networkPlayerProcessor.UpdatePlayerFromNetworkMessage(netMsg);
            }
            // Update Player Settings:
            while (true)
            {
                Message netMsg = _incomingMessageQueue.ReadWholeMessage(MessageType.PlayerSettings);

                if (netMsg == null)
                    break;

                _networkPlayerProcessor.UpdatePlayerSettingsFromNetworkMessage(netMsg);
            }
        }

        #endregion

        void AddClientChatMessageToServerLog(Message netMsg)
        {
            ChatMessage chatMsg = (ChatMessage)netMsg.Data;

            chatMsg.ClientName = _clientStateTracker[netMsg.ClientID].PlayerSettings.Name;

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
