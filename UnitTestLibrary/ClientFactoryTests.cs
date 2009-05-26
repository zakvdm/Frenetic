﻿using System;
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
        PlayerView.Factory playerViewFactory;

        PlayerView createdPlayerView = new PlayerView(null, null, null, null, null);
        PlayerView createdLocalPlayerView = new PlayerView(null, null, null, null, null);
        Client createdClient = new Client(null, null);
        
        [SetUp]
        public void SetUp()
        {
            gameSession = new GameSession();
            localClient = new LocalClient(MockRepository.GenerateStub<IPlayer>(), null);
            playerViewFactory = player => { if (player == localClient.Player) return createdLocalPlayerView; else return createdPlayerView; };
            clientFactoryDelegate = () => createdClient;
            clientFactory = new ClientSideClientFactory(clientFactoryDelegate, gameSession, playerViewFactory, localClient);
        }
        [Test]
        public void AddsAPlayerViewToGameSessionViewAndSetsID()
        {
            Client newClient = clientFactory.MakeNewClient(100);

            Assert.IsTrue(gameSession.Views.Contains(createdPlayerView));
            Assert.AreEqual(100, newClient.ID);
        }

        [Test]
        public void AddsAViewForLocalClient()
        {
            clientFactory.GetLocalClient();

            Assert.AreEqual(localClient, clientFactory.GetLocalClient());
            Assert.AreEqual(1, gameSession.Views.Count);
            Assert.IsTrue(gameSession.Views.Contains(createdLocalPlayerView));
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ServerSideFactoryCantCreateLocalClient()
        {
            ServerSideClientFactory factory = new ServerSideClientFactory(null, null);

            factory.GetLocalClient();
        }

    }
}
