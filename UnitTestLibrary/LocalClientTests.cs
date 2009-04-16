using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LocalClientTests
    {
        [Test]
        public void CanBuildAndItPrettyMuchWorks()
        {
            // TODO: So far the implementation of LocalClient is identical to Client... I'm only seperating them because they need different autofac lifetimes...
            //   is there a better way to handle this?

            LocalClient localClient = new LocalClient(MockRepository.GenerateStub<IPlayer>(), MockRepository.GenerateStub<PlayerSettings>());

            Assert.IsNotNull(localClient.Player);
            Assert.IsNotNull(localClient.PlayerSettings);
            Assert.AreEqual(0, localClient.ID);
            Assert.AreEqual(1, localClient.LastClientSnap);
            Assert.AreEqual(1, localClient.LastServerSnap);
        }
    }
}
