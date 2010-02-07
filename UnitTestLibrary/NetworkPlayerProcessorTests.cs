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

        // PlayerInput
        [Test]
        public void UpdatePlayerFromPlayerInputWorksCorrectly()
        {
            var stubPlayerInput = MockRepository.GenerateStub<IPlayerInput>();
            stubPlayerInput.PendingShot = new Vector2(100, 200);
            stubPlayerInput.Position = new Vector2(300, 400);

            networkPlayerController.UpdatePlayerFromNetworkItem(new Item() { ClientID = 10, Type = ItemType.PlayerInput, Data = stubPlayerInput });

            stubPlayerInput.AssertWasCalled(me => me.RefreshPlayerValuesFromInput(clientStateTracker.FindNetworkClient(10).Player));
        }

        // PlayerState
        [Test]
        public void UpdatePlayerFromPlayerStateItemWorksCorrectly()
        {
            var stubPlayerState = MockRepository.GenerateStub<IPlayerState>();
            stubPlayerState.Position = new Vector2(1, 2);
            stubPlayerState.NewShots = new List<Frenetic.Gameplay.Weapons.Shot>();
            stubPlayerState.NewShots.Add(new Frenetic.Gameplay.Weapons.Shot());

            networkPlayerController.UpdatePlayerFromPlayerStateItem(new Item() { ClientID = 10, Type = ItemType.Player, Data = stubPlayerState });

            stubPlayerState.AssertWasCalled(me => me.RefreshPlayerValuesFromState(clientStateTracker.FindNetworkClient(10).Player));
        }
        [Test]
        public void UpdatesLocalPlayerFromPlayerStateItem()
        {
            var stubPlayerState = MockRepository.GenerateStub<IPlayerState>();
            stubPlayerState.Position = new Vector2(1, 2);
            stubPlayerState.NewShots = new List<Frenetic.Gameplay.Weapons.Shot>();
            stubPlayerState.NewShots.Add(new Frenetic.Gameplay.Weapons.Shot());

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
