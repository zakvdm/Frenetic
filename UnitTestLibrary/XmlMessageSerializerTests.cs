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
            Message msg = new Message() { Type = MessageType.NewClient, Data = 10 };

            byte[] serializedMessage = serializer.Serialize(msg);

            Assert.AreEqual(MessageType.NewClient, (serializer.Deserialize(serializedMessage)).Type);
            Assert.AreEqual(10, (serializer.Deserialize(serializedMessage)).Data);
        }

        [Test]
        public void CanSerialiseAndDeserialisePlayerPosition()
        {
            Player player = new Player(null, null, null, null, null);
            player.Position = new Vector2(100, 200);
            Message msg = new Message() { Type = MessageType.Player, Data = player };

            byte[] serializedMsg = serializer.Serialize(msg);
            Player recoveredPlayer = (Player)(serializer.Deserialize(serializedMsg)).Data;

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
            Message msg = new Message() { Type = MessageType.Player, Data = state };

            byte[] serializedMessage = serializer.Serialize(msg);
            PlayerState recoveredState = (PlayerState)(serializer.Deserialize(serializedMessage)).Data;

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
