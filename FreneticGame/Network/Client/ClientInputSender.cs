using System;
using Lidgren.Network;

namespace Frenetic.Network
{
    public class ClientInputSender : IView
    {
        public ClientInputSender(Client localClient, IMessageConsole messageConsole, IOutgoingMessageQueue outgoingMessageQueue)
        {
            _localClient = localClient;
            _messageConsole = messageConsole;
            _outgoingMessageQueue = outgoingMessageQueue;
        }

        #region IView Members

        public void Generate()
        {
            // If the client ID is still 0 then we can't be connected yet, so no point in trying to send...
            if (_localClient.ID == 0)
                return;

            // Send the last received server snap
            _outgoingMessageQueue.Write(new Message() { ClientID = _localClient.ID, Type = MessageType.ServerSnap, Data = _localClient.LastServerSnap });

            while (_messageConsole.HasNewMessages)
            {
                string message = _messageConsole.GetNewMessage();
                _outgoingMessageQueue.Write(new Message() { ClientID = _localClient.ID, Type = MessageType.ChatLog, Data = message });
            }
        }

        int tmpcount = 0;

        #endregion

        IMessageConsole _messageConsole;
        IOutgoingMessageQueue _outgoingMessageQueue;
        Client _localClient;
    }
}
