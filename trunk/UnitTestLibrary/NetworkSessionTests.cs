using System;
using System.Collections.Generic;
using System.Text;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetworkSessionTests
    {
        MockRepository mocks;
        INetworkServer mockNetworkServer;
        INetworkClient mockNetworkClient;

        LidgrenNetworkSession clientSession, serverSession;

        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            mocks = new MockRepository();
            mockNetworkServer = mocks.StrictMock<INetworkServer>();
            mockNetworkClient = mocks.StrictMock<INetworkClient>();

            clientSession = new LidgrenNetworkSession(mockNetworkClient);
            serverSession = new LidgrenNetworkSession(mockNetworkServer);
        }

        [SetUp]
        public void Setup()
        {
            mocks.BackToRecordAll();
        }

        [Test]
        public void CanInstantiateNetworkSession()
        {
            Assert.IsNotNull(clientSession);
            Assert.IsNotNull(serverSession);

            Assert.IsTrue(serverSession.IsServer);
            Assert.IsFalse(clientSession.IsServer);
        }

        [Test]
        [ExpectedException( ExceptionType = typeof(System.InvalidOperationException), 
                            ExpectedMessage = "Client can't start session")]
        public void CantCreateSessionAsClient()
        {
            clientSession.Create();
        }

        [Test]
        public void ServerCanCreateSessionCorrectly()
        {
            using (mocks.Record())
            {
                mockNetworkServer.Start();
                Expect.Call(mockNetworkServer.IsListening).Return(false);
            }
            using (mocks.Playback())
            {
                serverSession.Create();
            }
        }

        [Test]
        [ExpectedException( ExceptionType = typeof(System.InvalidOperationException),
                            ExpectedMessage = "Session already created")]
        public void ServerCantBeStartedTwice()
        {
            LidgrenNetworkSession session = new LidgrenNetworkSession(mockNetworkServer);

            using (mocks.Record())
            {
                mockNetworkServer.Start();
                Expect.Call(mockNetworkServer.IsListening).Return(true);
            }
            using (mocks.Playback())
            {
                session.Create();
            }
        }

        [Test]
        public void CanShutdownSession()
        {
            using (mocks.Record())
            {
                using (mocks.Ordered())
                {
                    mockNetworkClient.Shutdown("shutdown");
                    mockNetworkServer.Shutdown("shutdown");
                }
            }
            using (mocks.Playback())
            {
                clientSession.Shutdown("shutdown");
                serverSession.Shutdown("shutdown");
            }
        }

        [Test]
        [ExpectedException(ExceptionType=typeof(System.InvalidOperationException), 
                            ExpectedMessage="Server can't join session")]
        public void OnlyClientCanJoinSession()
        {
            serverSession.Join("IP", 1234);
        }

        [Test]
        public void ClientCanJoinSessionCorrectly()
        {
            Assert.IsTrue(false);
        }
    }
}
