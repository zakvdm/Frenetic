using System;
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
        bool clientFactoryWasUsed;
        Client.Factory clientFactory;

        [SetUp]
        public void SetUp()
        {
            clientFactoryWasUsed = false;
            clientFactory = () => { clientFactoryWasUsed = true; return new Client(null, null); };
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            clientStateTracker = new ClientStateTracker(stubSnapCounter, clientFactory);
        }


        [Test]
        public void AddNewClientUsesClientFactory()
        {
            clientStateTracker.AddNewClient(100);

            Assert.IsTrue(clientFactoryWasUsed);
            Assert.IsNotNull(clientStateTracker[100]);
        }

        [Test]
        public void NewClientsAddedWithLatestServerSnap()
        {
            stubSnapCounter.CurrentSnap = 12;
            clientStateTracker.AddNewClient(100);

            foreach (Client client in clientStateTracker.CurrentClients)
            {
                Assert.AreEqual(12, client.LastServerSnap);
            }
        }

        [Test]
        public void CanIndexBasedOnClientID()
        {
            clientStateTracker.AddNewClient(100);

            Assert.AreEqual(100, clientStateTracker[100].ID);
        }

        [Test]
        public void CanSetLastSnapViaIDIndex()
        {
            clientStateTracker.AddNewClient(12);

            clientStateTracker[12].LastServerSnap = 1823;

            Assert.AreEqual(1823, clientStateTracker[12].LastServerSnap);
        }
    }
}
