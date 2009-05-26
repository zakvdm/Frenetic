using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Frenetic.Network
{
    public class IncomingMessageQueue : IIncomingMessageQueue
    {
        public IncomingMessageQueue(INetworkSession networkSession)
        {
            _networkSession = networkSession;
            InitializeQueues();
        }

        public bool HasAvailable(MessageType type)
        {
            EnqueueAllWaitingMessagesFromNetworkSession();

            if (_data[type].Count == 0)
                return false;

            return true;
        }

        public Message ReadWholeMessage(MessageType type)
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
        }

        Dictionary<MessageType, Queue<Message>> _data = new Dictionary<MessageType,Queue<Message>>();
        INetworkSession _networkSession;
    }
}
