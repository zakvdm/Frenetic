using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Network;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetworkPlayerProcessorTests
    {
        ClientStateTracker clientStateTracker;
        NetworkPlayerProcessor networkPlayerController;
        
        [SetUp]
        public void SetUp()
        {
            clientStateTracker = new ClientStateTracker(MockRepository.GenerateStub<ISnapCounter>(), () => new Client(new Player(null, null), new NetworkPlayerSettings()));
            networkPlayerController = new NetworkPlayerProcessor(clientStateTracker);
            clientStateTracker.AddNewClient(10);
        }

        [Test]
        public void HandlesNonExistentClientIDWithoutComplaining()
        {
            networkPlayerController.UpdatePlayerFromNetworkMessage(new Message() { ClientID = 11, Type = MessageType.Player, Data = new Player(null, null) });
        }


        [Test]
        public void UpdatesPositionBasedOnMessage()
        {
            Player receivedPlayer = new Player(null, null);
            receivedPlayer.Position = new Vector2(100, 200);
            clientStateTracker[10].Player.Position = Vector2.Zero;

            networkPlayerController.UpdatePlayerFromNetworkMessage(new Message() { ClientID = 10, Type = MessageType.Player, Data = receivedPlayer });

            Assert.AreEqual(new Vector2(100, 200), clientStateTracker[10].Player.Position);
        }

        [Test]
        public void UpdatesPlayerSettingsBasedOnMessage()
        {
            NetworkPlayerSettings receivedPlayerSettings = new NetworkPlayerSettings() { Name = "Test Name" };
            clientStateTracker[10].PlayerSettings.Name = "Nom de Plume";

            networkPlayerController.UpdatePlayerSettingsFromNetworkMessage(new Message() { ClientID = 10, Type = MessageType.PlayerSettings, Data = receivedPlayerSettings });

            Assert.AreEqual("Test Name", clientStateTracker[10].PlayerSettings.Name);
        }
    }
}
