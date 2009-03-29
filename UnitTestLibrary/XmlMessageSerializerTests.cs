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
        XmlMessageSerializer serializer;
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            serializer = new XmlMessageSerializer();
        }

        [Test]
        public void CanSerializeAndDeserializeAMessage()
        {
            Message msg = new Message() { Type = MessageType.NewPlayer, Data = 10 };

            byte[] serializedMessage = serializer.Serialize(msg);

            Assert.AreEqual(MessageType.NewPlayer, (serializer.Deserialize(serializedMessage)).Type);
            Assert.AreEqual(10, (serializer.Deserialize(serializedMessage)).Data);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithPlayerAsData()
        {
            Player player = new Player(10, null, null, null);
            Message msg = new Message() { Type = MessageType.PlayerData, Data = player };

            byte[] serializedMessage = serializer.Serialize(msg);

            Message recoveredMessage = serializer.Deserialize(serializedMessage);
            Assert.AreEqual(10, ((Player)recoveredMessage.Data).ID);
        }

        [Test]
        public void SerializesPlayerSettingsAlongWithPlayerData()
        {
            PlayerSettings playerSettings = new PlayerSettings();
            playerSettings.Name = "Jean Pant";
            Player player = new Player(10, playerSettings, null, null);
            Message msg = new Message() { Type = MessageType.PlayerData, Data = player };

            byte[] serializedMessage = serializer.Serialize(msg);

            Message recoveredMessage = serializer.Deserialize(serializedMessage);
            Assert.AreEqual("Jean Pant", ((Player)recoveredMessage.Data).Settings.Name);
        }
    }
}
