using System;
using Lidgren.Network;

namespace Frenetic.Network
{
    public class OutgoingMessageQueue : IOutgoingMessageQueue
    {
        public OutgoingMessageQueue(IClientNetworkSession clientNetworkSession, IServerNetworkSession serverNetworkSession)
        {
            _clientNetworkSession = clientNetworkSession;
            _serverNetworkSession = serverNetworkSession;
        }
        
        #region IOutgoingMessageQueue Members

        public void Write(Message message)
        {
            Write(message, NetChannel.UnreliableInOrder1);
        }

        // TODO: Make private
        public void Write(Message message, NetChannel channel)
        {
            if (_clientNetworkSession != null)
            {
                _clientNetworkSession.SendToServer(message, channel);
            }
            if (_serverNetworkSession != null)
            {
                _serverNetworkSession.SendToAll(message, channel);
            }
        }

        public void WriteFor(Message message, Client destinationClient)
        {
            WriteFor(message, NetChannel.Unreliable, destinationClient.ID);
        }

        // TODO: Make private...
        public void WriteFor(Message message, NetChannel channel, int destinationPlayerID)
        {
            _serverNetworkSession.SendTo(message, channel, destinationPlayerID);
        }

        // TODO: DELETE:
        public void WriteForAllExcept(Message message, NetChannel channel, int excludedPlayerID)
        {
            _serverNetworkSession.SendToAllExcept(message, channel, excludedPlayerID);
        }

        #endregion

        IClientNetworkSession _clientNetworkSession;
        IServerNetworkSession _serverNetworkSession;
    }
}
