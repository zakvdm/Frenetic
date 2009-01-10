using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Frenetic
{
    public class MessageQueue
    {
        INetworkSession _networkSession;
        Dictionary<MessageType, Queue<object>> _data = new Dictionary<MessageType,Queue<object>>();
        XmlSerializer _serializer = new XmlSerializer(typeof(Message));
        public MessageQueue(INetworkSession networkSession)
        {
            _networkSession = networkSession;
            foreach (MessageType type in Enum.GetValues(typeof(MessageType)))
            {
                _data.Add(type, new Queue<object>());
            }
        }

        public virtual object ReadMessage(MessageType type)
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
    }
}
