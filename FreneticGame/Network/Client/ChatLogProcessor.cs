using System;
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
            while (true)
            {
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ServerSnap);

                if (message == null)
                    break;

                _localClient.LastServerSnap = (int)message.Data;
            }

            // Set last acknowledged client snap:
            while (true)
            {
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ClientSnap);

                if (message == null)
                    break;

                _localClient.LastClientSnap = (int)message.Data;
            }

            // update chat log from server:
            while (true)
            {
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ChatLog);

                if (message == null)
                    break;

                AddChatMessage((ChatMessage)message.Data);
            }
            // update the players:
            while (true)
            {
                Message netMsg = _incomingMessageQueue.ReadWholeMessage(MessageType.Player);

                if (netMsg == null)
                    break;

                _networkPlayerProcessor.UpdatePlayerFromNetworkMessage(netMsg);
            }
            // update the player settings:
            while (true)
            {
                Message netMsg = _incomingMessageQueue.ReadWholeMessage(MessageType.PlayerSettings);

                if (netMsg == null)
                    break;

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
