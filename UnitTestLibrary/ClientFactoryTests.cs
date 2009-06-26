using System;
using Frenetic.Network;
using NUnit.Framework;
using Frenetic;
using Frenetic.Player;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientFactoryTest
    {
        GameSession gameSession;
        ClientSideClientFactory clientFactory;
        LocalClient localClient;
        Client.Factory clientFactoryDelegate;
        PlayerView playerView;
        Client createdClient = new Client(MockRepository.GenerateStub<IPlayer>(), MockRepository.GenerateStub<IPlayerSettings>());
        
        [SetUp]
        public void SetUp()
        {
            gameSession = new GameSession();
            localClient = new LocalClient(MockRepository.GenerateStub<IPlayer>(), MockRepository.GenerateStub<LocalPlayerSettings>());
            playerView = new PlayerView(null, null, null, null);
            clientFactoryDelegate = () => createdClient;
            clientFactory = new ClientSideClientFactory(clientFactoryDelegate, gameSession, playerView, localClient);
        }
        [Test]
        public void AddsAPlayerToThePlayerViewAndSetsID()
        {
            Client newClient = clientFactory.MakeNewClient(100);

            Assert.IsTrue(playerView.Players.Contains(newClient.Player));
            Assert.AreEqual(100, newClient.ID);
        }

        [Test]
        public void AddsAViewForLocalClient()
        {
            clientFactory.GetLocalClient();

            Assert.AreEqual(localClient, clientFactory.GetLocalClient());
            Assert.AreEqual(1, playerView.Players.Count);
            Assert.IsTrue(playerView.Players.Contains(localClient.Player));
        }

        [Test]
        public void OnlyAddsViewForLocalClientOnce()
        {
            clientFactory.GetLocalClient();
            clientFactory.GetLocalClient();

            Assert.AreEqual(1, playerView.Players.Count);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ServerSideFactoryCantCreateLocalClient()
        {
            ServerSideClientFactory factory = new ServerSideClientFactory(null, null);

            factory.GetLocalClient();
        }

        [Test]
        public void RemovesViewWhenDeletingClient()
        {
            Client client = clientFactory.MakeNewClient(200);

            clientFactory.DeleteClient(client);

            Assert.AreEqual(0, playerView.Players.Count);
        }
    }

}
