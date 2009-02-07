using System;

using Frenetic;

using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using Frenetic.Network;
namespace UnitTestLibrary
{
    [TestFixture]
    public class XmlMessageSerializerTests
    {
        [Test]
        public void CanMake()
        {
            XmlMessageSerializer serializer = new XmlMessageSerializer();
            Assert.IsNotNull(serializer);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessage()
        {
            XmlMessageSerializer serializer = new XmlMessageSerializer();
            Message msg = new Message() { Type = MessageType.NewPlayer, Data = 10 };

            byte[] serializedMessage = serializer.Serialize(msg);

            Assert.AreEqual(MessageType.NewPlayer, (serializer.Deserialize(serializedMessage)).Type);
            Assert.AreEqual(10, (serializer.Deserialize(serializedMessage)).Data);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithPlayerAsData()
        {
            XmlMessageSerializer serializer = new XmlMessageSerializer();
            Player player = new Player(10, null, null);
            Message msg = new Message() { Type = MessageType.PlayerData, Data = player };

            byte[] serializedMessage = serializer.Serialize(msg);

            Message recoveredMessage = serializer.Deserialize(serializedMessage);
            Assert.AreEqual(10, ((Player)recoveredMessage.Data).ID);
        }
    }
}
