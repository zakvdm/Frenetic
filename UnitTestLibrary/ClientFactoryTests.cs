using System;
using Frenetic.Network;
using NUnit.Framework;
using Frenetic;
using Frenetic.Player;
using Rhino.Mocks;
using Frenetic.Gameplay.Weapons;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientFactoryTest
    {
        PlayerList playerList;
        ClientSideClientFactory clientFactory;
        LocalClient localClient;
        Client.Factory clientFactoryDelegate;
        Client createdClient = new Client(MockRepository.GenerateStub<IPlayer>());
        
        [SetUp]
        public void SetUp()
        {
            playerList = new PlayerList();
            localClient = new LocalClient(MockRepository.GenerateStub<IPlayer>());
            clientFactoryDelegate = () => createdClient;
            clientFactory = new ClientSideClientFactory(clientFactoryDelegate, playerList, localClient);
        }

        [Test]
        public void ServerSideFactoryAddsPlayersToPlayerList()
        {
            ServerSideClientFactory serverClientFactory = new ServerSideClientFactory(clientFactoryDelegate, playerList);

            Client newClient = serverClientFactory.MakeNewClient(300);

            Assert.IsTrue(playerList.Players.Contains(newClient.Player));
            Assert.AreEqual(300, newClient.ID);
        }
        [Test]
        public void ServerSideFactoryRemovesPlayerFromPlayerListWhenDeletingClient()
        {
            ServerSideClientFactory serverClientFactory = new ServerSideClientFactory(clientFactoryDelegate, playerList);
            Client client = serverClientFactory.MakeNewClient(300);

            serverClientFactory.DeleteClient(client);

            Assert.AreEqual(0, playerList.Players.Count);
        }

        [Test]
        public void AddsLocalClientToPlayerList()
        {
            clientFactory.GetLocalClient();

            Assert.AreEqual(localClient, clientFactory.GetLocalClient());
            Assert.AreEqual(1, playerList.Players.Count);
            Assert.IsTrue(playerList.Players.Contains(localClient.Player));
        }

        [Test]
        public void OnlyAddsPlayerForLocalClientOnce()
        {
            clientFactory.GetLocalClient();
            clientFactory.GetLocalClient();

            Assert.AreEqual(1, playerList.Players.Count);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ServerSideFactoryCantCreateLocalClient()
        {
            ServerSideClientFactory factory = new ServerSideClientFactory(null, new PlayerList());

            factory.GetLocalClient();
        }

        [Test]
        public void RemovesPlayerFromPlayerListWhenDeletingClientOnClientSide()
        {
            Client client = clientFactory.MakeNewClient(200);

            clientFactory.DeleteClient(client);

            Assert.AreEqual(0, playerList.Players.Count);
        }
    }

}
