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
        [Test]
        public void NewClientsAddedWithLatestServerSnap()
        {
            ISnapCounter stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubSnapCounter.CurrentSnap = 12;
            ClientStateTracker clientStateTracker = new ClientStateTracker(stubSnapCounter);
            clientStateTracker.AddNewClient(100);

            foreach (Client client in clientStateTracker.CurrentClients)
            {
                Assert.AreEqual(12, client.LastServerSnap);
            }
        }

        [Test]
        public void CanIndexBasedOnClientID()
        {
            ClientStateTracker clientStateTracker = new ClientStateTracker(MockRepository.GenerateStub<ISnapCounter>());
            clientStateTracker.AddNewClient(100);

            Assert.AreEqual(100, clientStateTracker[100].ID);
        }

        [Test]
        public void CanSetLastSnapViaIDIndex()
        {
            ClientStateTracker clientStateTracker = new ClientStateTracker(MockRepository.GenerateStub<ISnapCounter>());
            clientStateTracker.AddNewClient(12);

            clientStateTracker[12].LastServerSnap = 1823;

            Assert.AreEqual(1823, clientStateTracker[12].LastServerSnap);
        }
    }
}
