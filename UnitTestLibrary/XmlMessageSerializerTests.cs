using System;

using Frenetic;

using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using Frenetic.Network;
using Microsoft.Xna.Framework;
using Frenetic.Player;
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
            Message msg = new Message() { Type = MessageType.NewClient, Data = 10 };

            byte[] serializedMessage = serializer.Serialize(msg);

            Assert.AreEqual(MessageType.NewClient, (serializer.Deserialize(serializedMessage)).Type);
            Assert.AreEqual(10, (serializer.Deserialize(serializedMessage)).Data);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithPlayerAsData()
        {
            Player player = new Player(null, null);
            player.Position = new Vector2(1, 2);
            Message msg = new Message() { Type = MessageType.Player, Data = player };

            byte[] serializedMessage = serializer.Serialize(msg);

            Message recoveredMessage = serializer.Deserialize(serializedMessage);
            Assert.AreEqual(new Vector2(1, 2), ((Player)recoveredMessage.Data).Position);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithNetworkPlayerSettingsAsData()
        {
            NetworkPlayerSettings playerSettings = new NetworkPlayerSettings();
            playerSettings.Name = "Jean Pant";
            Message msg = new Message() { Type = MessageType.PlayerSettings, Data = playerSettings };

            byte[] serializedMessage = serializer.Serialize(msg);

            Message recoveredMessage = serializer.Deserialize(serializedMessage);
            Assert.AreEqual("Jean Pant", ((NetworkPlayerSettings)recoveredMessage.Data).Name);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithLocalPlayerSettingsAsData()
        {
            LocalPlayerSettings playerSettings = new LocalPlayerSettings();
            playerSettings.Name = "Jean Pant";
            Message msg = new Message() { Type = MessageType.PlayerSettings, Data = playerSettings };

            byte[] serializedMessage = serializer.Serialize(msg);

            Message recoveredMessage = serializer.Deserialize(serializedMessage);
            Assert.AreEqual("Jean Pant", ((IPlayerSettings)recoveredMessage.Data).Name);
        }
    }
}
