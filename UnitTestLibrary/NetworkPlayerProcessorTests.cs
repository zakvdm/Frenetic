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
        Client localClient;
        
        [SetUp]
        public void SetUp()
        {
            client = new Client(MockRepository.GenerateStub<IPlayer>(), new NetworkPlayerSettings()) { ID = 10 };
            localClient = new Client(MockRepository.GenerateStub<IPlayer>(), new LocalPlayerSettings()) { ID = 99 };
            clientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            clientStateTracker.Stub(me => me.FindNetworkClient(10)).Return(client);
            clientStateTracker.Stub(me => me.LocalClient).Return(localClient);
            clientStateTracker.Stub(me => me.NetworkClients).Return(new List<Client>() { client });
            networkPlayerController = new NetworkPlayerProcessor(clientStateTracker);
        }

        [Test]
        public void HandlesNonExistentClientIDWithoutComplaining()
        {
            networkPlayerController.UpdatePlayerFromNetworkMessage(new Message() { ClientID = 11, Type = MessageType.Player, Data = MockRepository.GenerateStub<IPlayer>() });
            networkPlayerController.UpdatePlayerSettingsFromNetworkMessage(new Message() { ClientID = 11, Type = MessageType.PlayerSettings, Data = MockRepository.GenerateStub<IPlayerSettings>() });
        }

        [Test]
        public void UpdatesPositionBasedOnMessage()
        {
            var receivedPlayer = MockRepository.GenerateStub<IPlayer>();
            receivedPlayer.Position = new Vector2(100, 200);
            clientStateTracker.FindNetworkClient(10).Player.Position = Vector2.Zero;

            networkPlayerController.UpdatePlayerFromNetworkMessage(new Message() { ClientID = 10, Type = MessageType.Player, Data = receivedPlayer });

            Assert.AreEqual(new Vector2(100, 200), clientStateTracker.FindNetworkClient(10).Player.Position);
        }
        [Test]
        public void UpdatesPendingShotBasedOnMessage()
        {
            var receivedPlayer = MockRepository.GenerateStub<IPlayer>();
            receivedPlayer.PendingShot = Vector2.One;

            networkPlayerController.UpdatePlayerFromNetworkMessage(new Message() { ClientID = 10, Type = MessageType.Player, Data = receivedPlayer });

            Assert.AreEqual(Vector2.One, clientStateTracker.FindNetworkClient(10).Player.PendingShot);
        }

        // PlayerState
        [Test]
        public void UpdatePlayerFromPlayerStateMessageWorksCorrectly()
        {
            var stubPlayerState = MockRepository.GenerateStub<IPlayerState>();
            stubPlayerState.Position = new Vector2(1, 2);
            stubPlayerState.Shots = new List<Frenetic.Weapons.Shot>();
            stubPlayerState.Shots.Add(new Frenetic.Weapons.Shot());

            networkPlayerController.UpdatePlayerFromPlayerStateMessage(new Message() { ClientID = 10, Type = MessageType.Player, Data = stubPlayerState });

            stubPlayerState.AssertWasCalled(me => me.RefreshPlayerValuesFromState(clientStateTracker.FindNetworkClient(10).Player, PlayerType.Network));
        }
        [Test]
        public void DifferentiatesBetweenNetworkAndLocalPlayer()
        {
            var stubPlayerState = MockRepository.GenerateStub<IPlayerState>();
            stubPlayerState.Position = new Vector2(1, 2);
            stubPlayerState.Shots = new List<Frenetic.Weapons.Shot>();
            stubPlayerState.Shots.Add(new Frenetic.Weapons.Shot());

            networkPlayerController.UpdatePlayerFromPlayerStateMessage(new Message() { ClientID = 99, Type = MessageType.Player, Data = stubPlayerState });

            stubPlayerState.AssertWasCalled(me => me.RefreshPlayerValuesFromState(localClient.Player, PlayerType.Local));
        }

        // PlayerSettings
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
