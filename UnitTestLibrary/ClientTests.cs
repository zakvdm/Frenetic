using System;
using System.Collections.Generic;
using System.Text;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientTests
    {
        MockRepository mocks;
        INetworkClient mockNetworkClient;
        Client client;
        [SetUp]
        public void CreateAClient()
        {
            mocks = new MockRepository();
            mockNetworkClient = mocks.StrictMock<INetworkClient>();
            client = new Client(mockNetworkClient);
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void CheckThatClientIsStartedCorrectly()
        {
            string ip = "ip";
            int port = 0;
            using (mocks.Ordered())
            {
                mockNetworkClient.Start();
                mockNetworkClient.Connect(ip, port);
            }

            mocks.ReplayAll();

            client.Connect("ip", 0);
        }
    }
}
