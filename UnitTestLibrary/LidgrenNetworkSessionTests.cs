using System;
using System.Collections.Generic;
using System.Text;

using Frenetic.Network.Lidgren;
using Frenetic;

using Lidgren.Network;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LidgrenNetworkSessionTests
    {
        [Test]
        public void CanInstantiateNetworkSession()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetClient = MockRepository.GenerateStub<INetClient>();

            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);
            LidgrenNetworkSession clientNetworkSession = new LidgrenNetworkSession(stubNetClient);
            Assert.IsNotNull(clientNetworkSession);
            Assert.IsNotNull(serverNetworkSession);

            Assert.IsTrue(serverNetworkSession.IsServer);
            Assert.IsFalse(clientNetworkSession.IsServer);
        }

        [Test]
        [ExpectedException( ExceptionType = typeof(System.InvalidOperationException), 
                            ExpectedMessage = "Client can't start session")]
        public void CantCreateSessionAsClient()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            LidgrenNetworkSession clientNetworkSession = new LidgrenNetworkSession(stubNetClient);
            clientNetworkSession.Create();
        }

        [Test]
        public void ServerCanCreateSessionCorrectly()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);

            serverNetworkSession.Create();

            stubNetServer.AssertWasCalled(x => x.Start());
        }

        [Test]
        [ExpectedException( ExceptionType = typeof(System.InvalidOperationException),
                            ExpectedMessage = "Session already created")]
        public void ServerCantBeStartedTwice()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();

            LidgrenNetworkSession session = new LidgrenNetworkSession(stubNetServer);
            stubNetServer.Stub(x => x.IsListening).Return(true);

            session.Create();
        }

        [Test]
        public void CanShutdownSession()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetClient = MockRepository.GenerateStub<INetClient>();

            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);
            LidgrenNetworkSession clientNetworkSession = new LidgrenNetworkSession(stubNetClient);

            serverNetworkSession.Shutdown("shutdown");
            clientNetworkSession.Shutdown("shutdown");

            stubNetServer.AssertWasCalled(x => x.Shutdown("shutdown"));
            stubNetClient.AssertWasCalled(x => x.Shutdown("shutdown"));
        }

        [Test]
        [ExpectedException(ExceptionType=typeof(System.InvalidOperationException), 
                            ExpectedMessage="Server can't join session")]
        public void OnlyClientCanJoinSession()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);
            serverNetworkSession.Join("IP", 1234);
        }

        [Test]
        public void ClientCanJoinSessionCorrectlyWithIP()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            string ip = "ip";
            int port = 1;

            LidgrenNetworkSession clientNetworkSession = new LidgrenNetworkSession(stubNetClient);

            clientNetworkSession.Join(ip, port);

            stubNetClient.AssertWasCalled(x => x.Start());
            stubNetClient.AssertWasCalled(x => x.Connect(ip, port));
        }

        [Test]
        public void ClientCanSearchForLocalSessions()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            int port = 1;

            LidgrenNetworkSession clientNetworkSession = new LidgrenNetworkSession(stubNetClient);

            clientNetworkSession.Join(port);

            stubNetClient.AssertWasCalled(x => x.Start());
            stubNetClient.AssertWasCalled(x => x.DiscoverLocalServers(port));
        }

        [Test]
        public void ServerApprovesClientCorrecly()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);

            NetBuffer tmpBuffer = new NetBuffer();
            tmpBuffer.Write("hail data from client");

            stubNetServer.Stub(x => x.CreateBuffer()).Return(tmpBuffer);
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Equal(tmpBuffer), 
                    out Arg<NetMessageType>.Out(NetMessageType.ConnectionApproval).Dummy, 
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            serverNetworkSession.ReadMessage();

            stubNetConnection.AssertWasCalled(x => x.Approve());
        }

        [Test]
        public void ClientConnectsToFoundServer()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            LidgrenNetworkSession clientNetworkSession = new LidgrenNetworkSession(stubNetClient);
            NetBuffer tmp = new NetBuffer();

            stubNetClient.Stub(x => x.CreateBuffer()).Return(tmp);
            stubNetClient.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Equal(tmp),
                                            out Arg<NetMessageType>.Out(NetMessageType.ServerDiscovered).Dummy))
                                            .Return(true);
            stubNetClient.Stub(x => x.Connect(Arg<System.Net.IPEndPoint>.Is.Anything, Arg<byte[]>.Is.Anything));

            clientNetworkSession.ReadMessage();

            stubNetClient.AssertWasCalled(x => x.Connect(Arg<System.Net.IPEndPoint>.Is.Anything, Arg<byte[]>.Is.Anything));
        }

        [Test]
        public void ServerReceivesDataFromClientAndCreatesMessage()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            XmlMessageSerializer serializer = new XmlMessageSerializer();
            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);
            NetBuffer tmpBuffer = new NetBuffer();
            tmpBuffer.Write(serializer.Serialize(new Message() { Type = MessageType.PlayerData, Data = 10 }));

            stubNetConnection.Stub(x => x.ConnectionID).Return(1);
            stubNetServer.Stub(x => x.CreateBuffer()).Return(tmpBuffer);
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                            out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy, 
                            out Arg<INetConnection>.Out(stubNetConnection).Dummy))
                            .Return(true);

            Message msg = serverNetworkSession.ReadMessage();

            stubNetServer.AssertWasCalled(x => x.ReadMessage(Arg<NetBuffer>.Is.Equal(tmpBuffer), 
                                        out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy, 
                                        out Arg<INetConnection>.Out(stubNetConnection).Dummy));
            Assert.AreEqual(MessageType.PlayerData, msg.Type);
            Assert.AreEqual(10, msg.Data);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                            ExpectedMessage = "Client not connected to server")]
        public void CantSendWhenNotConnected()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            LidgrenNetworkSession clientNS = new LidgrenNetworkSession(stubNetClient);

            clientNS.Send(new Message(), NetChannel.Unreliable);
        }

        [Test]
        public void ClientProcessesAndSendsToServer()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            LidgrenNetworkSession clientNetworkSession = new LidgrenNetworkSession(stubNetClient);

            NetBuffer tmpBuffer = new NetBuffer();
            Message msg = new Message() { Type = MessageType.PlayerData, Data = 10 };
            stubNetClient.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);
            stubNetClient.Stub(x => x.Connected).Return(true);

            clientNetworkSession.Send(msg, NetChannel.ReliableUnordered);

            stubNetClient.AssertWasCalled(x => x.CreateBuffer(Arg<int>.Is.Anything));
            stubNetClient.AssertWasCalled(x => x.SendMessage(tmpBuffer, NetChannel.ReliableUnordered));

            byte[] data = (new XmlMessageSerializer()).Serialize(msg);
            Assert.AreEqual(data, tmpBuffer.ReadBytes(data.Length));
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Client can't send to all")]
        public void OnlyServerCanSendToAll()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            LidgrenNetworkSession clientNS = new LidgrenNetworkSession(stubNetClient);

            clientNS.SendToAll(null, NetChannel.Unreliable);
        }

        [Test]
        public void ServerCanSendMessagesToAllClients()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            LidgrenNetworkSession serverNS = new LidgrenNetworkSession(stubNetServer);
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);

            serverNS.SendToAll(msg, NetChannel.Unreliable);

            stubNetServer.AssertWasCalled(x => x.SendToAll(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Anything));
            Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, (new XmlMessageSerializer()).Deserialize(tmpBuffer.ToArray()).Data);
        }

        [Test]
        public void ServerApprovesNewConnection()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);

            stubNetServer.Stub(x => x.CreateBuffer()).Return(new NetBuffer("test"));
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.ConnectionApproval).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);
            
            Message msg = serverNetworkSession.ReadMessage();

            Assert.IsNull(msg);
            stubNetConnection.AssertWasCalled(x => x.Approve());
        }

        [Test]
        public void ServerCreatesNewPlayerMessageWhenConnectionStatusBecomesConnected()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            stubNetConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connected);
            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);

            stubNetServer.Stub(x => x.CreateBuffer()).Return(new NetBuffer("test"));
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);
            stubNetConnection.Stub(x => x.ConnectionID).Return(100);

            Message msg = serverNetworkSession.ReadMessage();

            Assert.AreEqual(MessageType.NewPlayer, msg.Type);
            Assert.AreEqual(100, (int)msg.Data);
        }
        [Test]
        public void ServerCreatesNewPlayerMessageONLYoncePerNewConnection()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            stubNetConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connected);
            LidgrenNetworkSession serverNetworkSession = new LidgrenNetworkSession(stubNetServer);

            stubNetServer.Stub(x => x.CreateBuffer()).Return(new NetBuffer("test"));
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);
            stubNetConnection.Stub(x => x.ConnectionID).Return(100);

            Message msg = serverNetworkSession.ReadMessage();
            Assert.IsNotNull(msg);
            msg = serverNetworkSession.ReadMessage();

            Assert.IsNull(msg);
        }

        #region stupid test of my C# knowledge
        class TempContainer
        { }
        [Test]
        public void CanNewAnOutParameter()
        {
            TempContainer test = new TempContainer();
            MethodToTest(out test);

            Assert.AreEqual(container, test);
        }
        TempContainer container = new TempContainer();
        private void MethodToTest(out TempContainer tmp)
        {
            tmp = container;
        }
        #endregion
    }
}
