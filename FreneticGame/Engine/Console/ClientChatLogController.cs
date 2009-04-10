using System;
using Frenetic.Network;

namespace Frenetic
{
    public class ClientChatLogController : IController
    {
        public ClientChatLogController(Client localClient, MessageLog chatLog, IIncomingMessageQueue incomingMessageQueue)
        {
            _localClient = localClient;
            _chatLog = chatLog;
            _incomingMessageQueue = incomingMessageQueue;
        }

        #region IController Members

        public void Process(float elapsedTime)
        {
            while (true)
            {
                // Get most recent server snap
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ServerSnap);

                if (message == null)
                    break;

                _localClient.LastServerSnap = (int)message.Data;
            }

            while (true)
            {
                // update chat log from server
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ChatLog);

                if (message == null)
                    break;

                _chatLog.AddMessage((string)message.Data);
            }
        }

        #endregion

        Client _localClient;
        MessageLog _chatLog;
        IIncomingMessageQueue _incomingMessageQueue;
    }
}
