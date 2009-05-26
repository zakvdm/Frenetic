﻿using System;
using NUnit.Framework;
using Frenetic.Network;
using Frenetic;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientStateTrackerTests
    {
        ISnapCounter stubSnapCounter;
        ClientStateTracker clientStateTracker;
        IClientFactory clientFactory;
        INetworkSession stubNetworkSession;

        [SetUp]
        public void SetUp()
        {
            clientFactory = MockRepository.GenerateStub<IClientFactory>();
            clientFactory.Stub(x => x.MakeNewClient(Arg<int>.Is.Anything)).Return(new Client(null, null) { ID = 100 });
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubNetworkSession = MockRepository.GenerateStub<INetworkSession>();
            clientStateTracker = new ClientStateTracker(stubSnapCounter, stubNetworkSession, clientFactory);
        }

        [Test]
        public void RegistersWithNetworkSessionForClientJoinedEvent()
        {
            stubNetworkSession.AssertWasCalled(me => me.ClientJoined += Arg<EventHandler<ClientJoinedEventArgs>>.Is.Anything);
        }

        [Test]
        public void GetsLocalClientFromFactoryAndSetsID()
        {
            clientFactory.Stub(x => x.GetLocalClient()).Return(new LocalClient(null, null));

            stubNetworkSession.Raise(me => me.ClientJoined += null, this, new ClientJoinedEventArgs(200, true));

            clientFactory.AssertWasNotCalled(x => x.MakeNewClient(Arg<int>.Is.Anything));
            clientFactory.AssertWasCalled(x => x.GetLocalClient());
            Assert.AreEqual(200, clientStateTracker.LocalClient.ID);
        }

        [Test]
        public void AddNewNetworkClientUsesClientFactory()
        {
            stubNetworkSession.Raise(me => me.ClientJoined += null, this, new ClientJoinedEventArgs(100, false));

            clientFactory.AssertWasNotCalled(x => x.GetLocalClient());
            clientFactory.AssertWasCalled(x => x.MakeNewClient(100));
            Assert.IsNotNull(clientStateTracker.FindNetworkClient(100));
        }

        [Test]
        public void NewClientsAddedWithLatestServerSnap()
        {
            stubSnapCounter.CurrentSnap = 12;
            stubNetworkSession.Raise(me => me.ClientJoined += null, this, new ClientJoinedEventArgs(100, false));

            foreach (Client client in clientStateTracker.NetworkClients)
            {
                Assert.AreEqual(12, client.LastServerSnap);
            }
        }

        [Test]
        public void CanFindAddedNetworkClients()
        {
            stubNetworkSession.Raise(me => me.ClientJoined += null, this, new ClientJoinedEventArgs(100, false));

            Assert.AreEqual(100, clientStateTracker.FindNetworkClient(100).ID);
        }

        [Test]
        public void CanSetLastSnapViaIDIndex()
        {
            stubNetworkSession.Raise(me => me.ClientJoined += null, this, new ClientJoinedEventArgs(100, false));

            clientStateTracker.FindNetworkClient(100).LastServerSnap = 1823;

            Assert.AreEqual(1823, clientStateTracker.FindNetworkClient(100).LastServerSnap);
        }

    }
}
