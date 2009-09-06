using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
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
            client = new Client(MockRepository.GenerateStub<IPlayer>()) { ID = 10 };
            localClient = new Client(MockRepository.GenerateStub<IPlayer>()) { ID = 99 };
            clientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            clientStateTracker.Stub(me => me.FindNetworkClient(10)).Return(client);
            clientStateTracker.Stub(me => me.LocalClient).Return(localClient);
            clientStateTracker.Stub(me => me.NetworkClients).Return(new List<Client>() { client });
            networkPlayerController = new NetworkPlayerProcessor(clientStateTracker);
        }

        [Test]
        public void HandlesNonExistentClientIDWithoutComplaining()
        {
            networkPlayerController.UpdatePlayerFromNetworkItem(new Item() { ClientID = 11, Type = ItemType.Player, Data = MockRepository.GenerateStub<IPlayer>() });
            networkPlayerController.UpdatePlayerSettingsFromNetworkItem(new Item() { ClientID = 11, Type = ItemType.PlayerSettings, Data = MockRepository.GenerateStub<IPlayerSettings>() });
        }

        [Test]
        public void UpdatesPositionBasedOnItem()
        {
            var receivedPlayer = MockRepository.GenerateStub<IPlayer>();
            receivedPlayer.Position = new Vector2(100, 200);
            clientStateTracker.FindNetworkClient(10).Player.Position = Vector2.Zero;

            networkPlayerController.UpdatePlayerFromNetworkItem(new Item() { ClientID = 10, Type = ItemType.Player, Data = receivedPlayer });

            Assert.AreEqual(new Vector2(100, 200), clientStateTracker.FindNetworkClient(10).Player.Position);
        }
        [Test]
        public void UpdatesPendingShotBasedOnItem()
        {
            var receivedPlayer = MockRepository.GenerateStub<IPlayer>();
            receivedPlayer.PendingShot = Vector2.One;

            networkPlayerController.UpdatePlayerFromNetworkItem(new Item() { ClientID = 10, Type = ItemType.Player, Data = receivedPlayer });

            Assert.AreEqual(Vector2.One, clientStateTracker.FindNetworkClient(10).Player.PendingShot);
        }

        // PlayerState
        [Test]
        public void UpdatePlayerFromPlayerStateItemWorksCorrectly()
        {
            var stubPlayerState = MockRepository.GenerateStub<IPlayerState>();
            stubPlayerState.Position = new Vector2(1, 2);
            stubPlayerState.NewShots = new List<Frenetic.Weapons.Shot>();
            stubPlayerState.NewShots.Add(new Frenetic.Weapons.Shot());

            networkPlayerController.UpdatePlayerFromPlayerStateItem(new Item() { ClientID = 10, Type = ItemType.Player, Data = stubPlayerState });

            stubPlayerState.AssertWasCalled(me => me.RefreshPlayerValuesFromState(clientStateTracker.FindNetworkClient(10).Player));
        }
        [Test]
        public void UpdatesLocalPlayerFromPlayerStateItem()
        {
            var stubPlayerState = MockRepository.GenerateStub<IPlayerState>();
            stubPlayerState.Position = new Vector2(1, 2);
            stubPlayerState.NewShots = new List<Frenetic.Weapons.Shot>();
            stubPlayerState.NewShots.Add(new Frenetic.Weapons.Shot());

            networkPlayerController.UpdatePlayerFromPlayerStateItem(new Item() { ClientID = 99, Type = ItemType.Player, Data = stubPlayerState });

            stubPlayerState.AssertWasCalled(me => me.RefreshPlayerValuesFromState(localClient.Player));
        }

        // PlayerSettings
        [Test]
        public void UpdatesPlayerSettingsBasedOnItem()
        {
            NetworkPlayerSettings receivedPlayerSettings = new NetworkPlayerSettings() { Name = "Test Name" };
            client.Player.Stub(me => me.PlayerSettings).Return(MockRepository.GenerateStub<IPlayerSettings>());
            client.Player.PlayerSettings.Name = "Nom de Plume";

            networkPlayerController.UpdatePlayerSettingsFromNetworkItem(new Item() { ClientID = 10, Type = ItemType.PlayerSettings, Data = receivedPlayerSettings });

            Assert.AreEqual("Test Name", clientStateTracker.FindNetworkClient(10).Player.PlayerSettings.Name);
        }
    }
}
