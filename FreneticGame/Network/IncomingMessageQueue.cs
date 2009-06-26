using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;

namespace Frenetic.Network
{
    public class IncomingMessageQueue : IIncomingMessageQueue
    {
        public IncomingMessageQueue(INetworkSession networkSession, IClientStateTracker clientStateTracker)
        {
            _networkSession = networkSession;
            _clientStateTracker = clientStateTracker;
            InitializeQueues();
        }

        public bool HasAvailable(MessageType type)
        {
            EnqueueAllWaitingMessagesFromNetworkSession();

            if (_data[type].Count == 0)
                return false;

            return true;
        }

        public Message ReadMessage(MessageType type)
        {
            EnqueueAllWaitingMessagesFromNetworkSession();

            if (_data[type].Count == 0)
                return null;

            return _data[type].Dequeue();
        }

        private void InitializeQueues()
        {
            // Make a queue for every possible message type
            foreach (MessageType type in Enum.GetValues(typeof(MessageType)))
            {
                _data.Add(type, new Queue<Message>());
            }
        }

        private void EnqueueAllWaitingMessagesFromNetworkSession()
        {
            // Read all incoming messages and sort them by MessageType
            Message msg;
            do
            {
                msg = _networkSession.ReadMessage();
                if (msg != null)
                {
                    _data[msg.Type].Enqueue(msg);
                }
            }
            while (msg != null);

            RemoveInvalidMessagesFromTheFrontOfEachQueue();
        }

        void RemoveInvalidMessagesFromTheFrontOfEachQueue()
        {
            // A message is invalid if we find no corresponding client in the ClientStateTracker (probably because the client who sent this message has since disconnected...)
            foreach (MessageType type in Enum.GetValues(typeof(MessageType)))
            {
                if (RequiresAValidClient(type))
                {
                    while (!MessageClientIsValid(type))
                    {
                        _data[type].Dequeue();
                    }
                }
            }
        }

        bool RequiresAValidClient(MessageType type)
        {
            return ((_data[type].Count > 0) && (_data[type].Peek().ClientID != 0));
        }
        bool MessageClientIsValid(MessageType type)
        {
            return ((_data[type].Count == 0)
                || (_clientStateTracker.FindNetworkClient(_data[type].Peek().ClientID) != null)
                || ((_clientStateTracker.LocalClient != null) && (_clientStateTracker.LocalClient.ID == _data[type].Peek().ClientID)));
        }

        Dictionary<MessageType, Queue<Message>> _data = new Dictionary<MessageType,Queue<Message>>();
        INetworkSession _networkSession;
        IClientStateTracker _clientStateTracker;
    }
}
