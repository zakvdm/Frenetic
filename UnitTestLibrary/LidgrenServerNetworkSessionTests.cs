using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network.Lidgren;
using Lidgren.Network;
using Frenetic.Network;
using Autofac.Builder;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LidgrenServerNetworkSessionTests
    {
        [Test]
        public void CanBuildNetworkSessionsWithAutofac()
        {
            var builder = new ContainerBuilder();
            builder.Register(MockRepository.GenerateStub<INetServer>()).SingletonScoped();
            builder.Register(MockRepository.GenerateStub<INetClient>()).SingletonScoped();
            builder.Register(x => new NetConfiguration("Frenetic")).FactoryScoped();
            builder.Register<LidgrenServerNetworkSession>();
            builder.Register<LidgrenClientNetworkSession>();
            builder.Register(MockRepository.GenerateStub<IMessageSerializer>()).SingletonScoped();

            var container = builder.Build();

            var serverNetworkSession = container.Resolve<LidgrenServerNetworkSession>();
            Assert.IsNotNull(serverNetworkSession);

            var clientNetworkSession = container.Resolve<LidgrenClientNetworkSession>();
            Assert.IsNotNull(clientNetworkSession);
        }

        XmlMessageSerializer _serializer = new XmlMessageSerializer();

        [Test]
        public void ServerCanCreateSessionCorrectly()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            LidgrenServerNetworkSession serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, null);

            serverNetworkSession.Create(20);

            Assert.AreEqual(20, stubNetServer.Port);
            stubNetServer.AssertWasCalled(x => x.Start());
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                            ExpectedMessage = "Session already created")]
        public void ServerCantBeStartedTwice()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();

            LidgrenServerNetworkSession session = new LidgrenServerNetworkSession(stubNetServer, null);
            stubNetServer.Stub(x => x.IsListening).Return(true);

            session.Create(1);
        }

        [Test]
        public void ServerApprovesClientCorrecly()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenServerNetworkSession serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, null);

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
        public void ServerReceivesDataFromClientAndCreatesMessage()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenServerNetworkSession serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, _serializer);
            NetBuffer tmpBuffer = new NetBuffer();
            tmpBuffer.Write(_serializer.Serialize(new Message() { Type = MessageType.PlayerData, Data = 10 }));

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
        public void ServerApprovesNewConnection()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenServerNetworkSession serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, null);

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
            LidgrenServerNetworkSession serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, null);

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
            LidgrenServerNetworkSession serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, null);

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

        // **************
        // SENDING TESTS:
        // **************
        [Test]
        public void ServerCanSendMessagesToAllClients()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            LidgrenServerNetworkSession serverNS = new LidgrenServerNetworkSession(stubNetServer, _serializer);
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);

            serverNS.SendToAll(msg, NetChannel.Unreliable);

            stubNetServer.AssertWasCalled(x => x.SendToAll(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Anything));
            Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, _serializer.Deserialize(tmpBuffer.ToArray()).Data);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Not a valid player")]
        public void CanOnlySendToValidConnections()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            stubNetServer.Stub(x => x.Connected).Return(true); // Simply means that *somebody* has joined (might not be the current connection we are sending to...)
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            stubConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connecting);
            LidgrenServerNetworkSession serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, _serializer);
            serverNetworkSession.ActiveConnections.Add(100, stubConnection);

            serverNetworkSession.SendTo(null, NetChannel.Unreliable, 100);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Not a valid player")]
        public void CantSendToNullConnections()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            stubNetServer.Stub(x => x.Connected).Return(true); // Simply means that *somebody* has joined (might not be the current connection we are sending to...)
            LidgrenServerNetworkSession serverNS = new LidgrenServerNetworkSession(stubNetServer, _serializer);

            serverNS.SendTo(null, NetChannel.Unreliable, 100);  // ID 100 not an active connection
        }

        [Test]
        public void ServerCanSendMessageToOneClient()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            stubConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connected);
            LidgrenServerNetworkSession serverNS = new LidgrenServerNetworkSession(stubNetServer, _serializer);
            serverNS.ActiveConnections.Add(100, stubConnection);
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);

            serverNS.SendTo(msg, NetChannel.Unreliable, 100);

            stubNetServer.AssertWasCalled(x => x.SendMessage(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Anything, Arg<INetConnection>.Is.Equal(stubConnection)));
            Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, _serializer.Deserialize(tmpBuffer.ToArray()).Data);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Excluded player not connected to network session")]
        public void SendToAllExceptConnectionCantBeNull()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            LidgrenServerNetworkSession serverNS = new LidgrenServerNetworkSession(stubNetServer, _serializer);

            serverNS.SendToAllExcept(null, NetChannel.Unreliable, 100); // 100 not an active connection
        }

        [Test]
        public void ServerCanSendMessageToAllExceptOneClient()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenServerNetworkSession serverNS = new LidgrenServerNetworkSession(stubNetServer, _serializer);
            serverNS.ActiveConnections.Add(100, stubConnection);
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);

            serverNS.SendToAllExcept(msg, NetChannel.Unreliable, 100);

            stubNetServer.AssertWasCalled(x => x.SendToAll(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Anything, Arg<INetConnection>.Is.Equal(stubConnection)));
            Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, _serializer.Deserialize(tmpBuffer.ToArray()).Data);
        }
    }
}
