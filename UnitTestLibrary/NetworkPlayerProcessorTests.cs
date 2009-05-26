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
        IClientStateTracker clientStateTracker;
        NetworkPlayerProcessor networkPlayerController;
        Client client;
        
        [SetUp]
        public void SetUp()
        {
            client = new Client(new Player(null, null), new NetworkPlayerSettings()) { ID = 10 };
            clientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            clientStateTracker.Stub(me => me.FindNetworkClient(10)).Return(client);
            clientStateTracker.Stub(me => me.NetworkClients).Return(new List<Client>() { client });
            networkPlayerController = new NetworkPlayerProcessor(clientStateTracker);
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
            clientStateTracker.FindNetworkClient(10).Player.Position = Vector2.Zero;

            networkPlayerController.UpdatePlayerFromNetworkMessage(new Message() { ClientID = 10, Type = MessageType.Player, Data = receivedPlayer });

            Assert.AreEqual(new Vector2(100, 200), clientStateTracker.FindNetworkClient(10).Player.Position);
        }

        [Test]
        public void UpdatesPlayerSettingsBasedOnMessage()
        {
            NetworkPlayerSettings receivedPlayerSettings = new NetworkPlayerSettings() { Name = "Test Name" };
            client.PlayerSettings.Name = "Nom de Plume";

            networkPlayerController.UpdatePlayerSettingsFromNetworkMessage(new Message() { ClientID = 10, Type = MessageType.PlayerSettings, Data = receivedPlayerSettings });

            Assert.AreEqual("Test Name", clientStateTracker.FindNetworkClient(10).PlayerSettings.Name);
        }
    }
}
