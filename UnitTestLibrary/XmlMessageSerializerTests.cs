using System;

using Frenetic;

using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using Frenetic.Network;
using Microsoft.Xna.Framework;
using Frenetic.Player;
using Rhino.Mocks;
using Frenetic.Weapons;
using System.Collections.Generic;
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
            Message msg = new Message() { Items = { new Item() { Type = ItemType.NewClient, Data = 10 } } };

            byte[] serializedMessage = serializer.Serialize(msg);

            Assert.AreEqual(ItemType.NewClient, (serializer.Deserialize(serializedMessage)).Items[0].Type);
            Assert.AreEqual(10, (serializer.Deserialize(serializedMessage)).Items[0].Data);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithChatMessages()
        {
            List<ChatMessage> chatLog = new List<ChatMessage>();
            chatLog.Add(new ChatMessage() { ClientName = "1", Message = "hello" });
            chatLog.Add(new ChatMessage() { ClientName = "2", Message = "baby" });
            Message msg = new Message() { Items = { new Item() { Type = ItemType.ChatLog, Data = chatLog } } };

            byte[] serializedMessage = serializer.Serialize(msg);

            Assert.AreEqual(ItemType.ChatLog, (serializer.Deserialize(serializedMessage)).Items[0].Type);
            Assert.AreEqual(2, ((List<ChatMessage>)((serializer.Deserialize(serializedMessage)).Items[0].Data)).Count);
        }

        [Test]
        public void CanSerialiseAndDeserialisePlayerPosition()
        {
            Player player = new Player(null, null, null, null, null);
            player.Position = new Vector2(100, 200);
            Message msg = new Message() { Items = { new Item() { Type = ItemType.Player, Data = player } } };

            byte[] serializedMsg = serializer.Serialize(msg);
            Player recoveredPlayer = (Player)(serializer.Deserialize(serializedMsg)).Items[0].Data;

            Assert.AreEqual(player.Position, recoveredPlayer.Position);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithPlayerState()
        {
            PlayerState state = new PlayerState();
            state.Position = new Vector2(100, 200);
            state.Shots = new List<Shot>();
            state.Shots.Add(new Shot(Vector2.UnitY, Vector2.UnitX));
            state.Score = new Frenetic.Gameplay.PlayerScore() { Deaths = 3, Kills = 4 };
            Message msg = new Message() { Items = { new Item() { Type = ItemType.Player, Data = state } } };

            byte[] serializedMessage = serializer.Serialize(msg);
            PlayerState recoveredState = (PlayerState)(serializer.Deserialize(serializedMessage)).Items[0].Data;

            Assert.AreEqual(new Vector2(100, 200), recoveredState.Position);
            Assert.AreEqual(1, recoveredState.Shots.Count);
            Assert.AreEqual(new Shot(Vector2.UnitY, Vector2.UnitX), recoveredState.Shots[0]);
            Assert.AreEqual(4, recoveredState.Score.Kills);
            Assert.AreEqual(3, recoveredState.Score.Deaths);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithNetworkPlayerSettingsAsData()
        {
            NetworkPlayerSettings playerSettings = new NetworkPlayerSettings();
            playerSettings.Name = "Jean Pant";
            Message msg = new Message() { Items = { new Item() { Type = ItemType.PlayerSettings, Data = playerSettings } } };

            byte[] serializedMessage = serializer.Serialize(msg);

            Message recoveredMessage = serializer.Deserialize(serializedMessage);
            Assert.AreEqual("Jean Pant", ((NetworkPlayerSettings)recoveredMessage.Items[0].Data).Name);
        }

        [Test]
        public void CanSerializeAndDeserializeAMessageWithLocalPlayerSettingsAsData()
        {
            LocalPlayerSettings playerSettings = new LocalPlayerSettings();
            playerSettings.Name = "Jean Pant";
            Message msg = new Message() { Items = { new Item() { Type = ItemType.PlayerSettings, Data = playerSettings } } };

            byte[] serializedMessage = serializer.Serialize(msg);

            Message recoveredMessage = serializer.Deserialize(serializedMessage);
            Assert.AreEqual("Jean Pant", ((IPlayerSettings)recoveredMessage.Items[0].Data).Name);
        }

        //[Test]
        public void PrintOutSerializedData()
        {
            XmlSerializer realSerializer = new XmlSerializer(typeof(Message));
            Player player = new Player(null, null, null, null, null);
            player.Position = new Vector2(100, 200);
            NetworkPlayerSettings playerSettings = new NetworkPlayerSettings() { Name = "test" };
            PlayerState state = new PlayerState() { Shots = new List<Shot>() { new Shot(), new Shot() } };
            var chatLog = new List<ChatMessage>();
            chatLog.Add(new ChatMessage() { ClientName = "1", Message = "hello" });

            var msg = new Message() { Items = { new Item() { ClientID = 1, Type = ItemType.Player, Data = player }, new Item() { ClientID = 2, Type = ItemType.PlayerSettings, Data = playerSettings }, new Item() { ClientID = 3, Type = ItemType.Player, Data = state }, new Item() { ClientID = 5, Data = chatLog } } };

            var stream = new MemoryStream();
            realSerializer.Serialize(stream, msg);
            stream.Position = 0;

            Console.WriteLine("Message is " + stream.ToArray().Length + " bytes long!");

            string tmp = new StreamReader(stream).ReadToEnd();
            Console.Write(tmp);
        }
    }
}
