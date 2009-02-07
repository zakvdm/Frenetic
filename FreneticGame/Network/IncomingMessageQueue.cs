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

        private void InitializeQueues()
        {
            foreach (MessageType type in Enum.GetValues(typeof(MessageType)))
            {
                _data.Add(type, new Queue<object>());
            }
        }

        public object ReadMessage(MessageType type)
        {
            ProcessMessages();

            if (_data[type].Count == 0)
                return null;

            return _data[type].Dequeue();
        }

        private void ProcessMessages()
        {
            Message msg;
            do
            {
                msg = _networkSession.ReadMessage();
                if (msg != null)
                {
                    _data[msg.Type].Enqueue(msg.Data);
                }
            }
            while (msg != null);
        }

        Dictionary<MessageType, Queue<object>> _data = new Dictionary<MessageType,Queue<object>>();
        INetworkSession _networkSession;
    }
}
