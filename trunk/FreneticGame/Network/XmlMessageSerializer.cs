using System;
using System.Xml.Serialization;
using System.IO;

namespace Frenetic
{
    public class XmlMessageSerializer : IMessageSerializer
    {
        XmlSerializer _serializer = new XmlSerializer(typeof(Message));

        #region IMessageSerializer Members
        public byte[] Serialize(Message msg)
        {
            MemoryStream stream = new MemoryStream();
            _serializer.Serialize(stream, msg);
            return stream.ToArray();
        }

        public Message Deserialize(byte[] data)
        {
            return (Message)_serializer.Deserialize(new MemoryStream(data));
        }

        #endregion
    }
}
