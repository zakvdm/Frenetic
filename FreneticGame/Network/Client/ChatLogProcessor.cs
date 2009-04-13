using System;
using Frenetic.Network;

namespace Frenetic
{
    public class ChatLogProcessor : IController
    {
        public ChatLogProcessor(Client localClient, Log<ChatMessage> chatLog, IIncomingMessageQueue incomingMessageQueue)
        {
            _localClient = localClient;
            _chatLog = chatLog;
            _incomingMessageQueue = incomingMessageQueue;
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
        }

        #endregion

        void AddChatMessage(ChatMessage newMsg)
        {
            // We don't want to add duplicate messages:
            if (_chatLog.Exists(msg => msg == newMsg))
                return;

            _chatLog.AddMessage(newMsg);
        }

        Client _localClient;
        Log<ChatMessage> _chatLog;
        IIncomingMessageQueue _incomingMessageQueue;
    }
}
