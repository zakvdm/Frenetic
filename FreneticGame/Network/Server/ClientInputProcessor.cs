using System;

namespace Frenetic.Network
{
    public class ClientInputProcessor : IController
    {
        public ClientInputProcessor(MessageLog serverChatLog, IClientStateTracker clientStateTracker, IIncomingMessageQueue incomingMessageQueue)
        {
            _serverChatLog = serverChatLog;
            _clientStateTracker = clientStateTracker;
            _incomingMessageQueue = incomingMessageQueue;
        }

        #region IController Members

        public void Process(float elapsedTime)
        {
            // Update the last received server snap:
            while (true)
            {
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ServerSnap);

                if (message == null)
                    break;

                _clientStateTracker[message.ClientID].LastServerSnap = (int)message.Data;
            }
            // Update server chat log:
            while (true)
            {
                Message message = _incomingMessageQueue.ReadWholeMessage(MessageType.ChatLog);

                if (message == null)
                    break;

                _serverChatLog.AddMessage((string)message.Data);
            }
        }

        #endregion

        MessageLog _serverChatLog;
        IClientStateTracker _clientStateTracker;
        IIncomingMessageQueue _incomingMessageQueue;
    }
}
