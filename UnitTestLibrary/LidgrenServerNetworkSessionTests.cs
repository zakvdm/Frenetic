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
        XmlMessageSerializer _serializer = new XmlMessageSerializer();

        INetServer stubNetServer;
        IServerMessageSender stubMessageSender;
        INetConnection stubNetConnection;
        LidgrenServerNetworkSession serverNetworkSession;
        [SetUp]
        public void SetUp()
        {
            stubNetServer = MockRepository.GenerateStub<INetServer>();
            stubMessageSender = MockRepository.GenerateStub<IServerMessageSender>();
            stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, stubMessageSender, _serializer);

            stubNetConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connected);
            stubNetConnection.Stub(x => x.ConnectionID).Return(100);
        }

        [Test]
        public void CanBuildNetworkSessionsWithAutofac()
        {
            var builder = new ContainerBuilder();
            builder.Register(MockRepository.GenerateStub<INetServer>()).SingletonScoped();
            builder.Register(MockRepository.GenerateStub<INetClient>()).SingletonScoped();
            builder.Register(MockRepository.GenerateStub<IServerMessageSender>()).SingletonScoped();
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


        [Test]
        public void ServerCanCreateSessionCorrectly()
        {
            serverNetworkSession.Create(20);

            Assert.AreEqual(20, stubNetServer.Port);
            stubNetServer.AssertWasCalled(x => x.Start());
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                            ExpectedMessage = "Session already created")]
        public void ServerCantBeStartedTwice()
        {
            stubNetServer.Stub(x => x.IsListening).Return(true);

            serverNetworkSession.Create(1);
        }

        
        [Test]
        public void ServerReceivesDataFromClientAndCreatesMessage()
        {
            NetBuffer tmpBuffer = new NetBuffer();
            tmpBuffer.Write(_serializer.Serialize(new Message() { Type = MessageType.Player, Data = 10 }));
            stubNetServer.Stub(x => x.CreateBuffer()).Return(tmpBuffer);
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                            out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy,
                            out Arg<INetConnection>.Out(stubNetConnection).Dummy))
                            .Return(true);

            Message msg = serverNetworkSession.ReadMessage();

            stubNetServer.AssertWasCalled(x => x.ReadMessage(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                        out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy,
                                        out Arg<INetConnection>.Out(stubNetConnection).Dummy));
            Assert.AreEqual(MessageType.Player, msg.Type);
            Assert.AreEqual(10, msg.Data);
        }

        // NEW CLIENT HANDLING:
        [Test]
        public void ServerApprovesNewConnection()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.ConnectionApproval).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            Message msg = serverNetworkSession.ReadMessage();

            Assert.IsNull(msg);
            stubNetConnection.AssertWasCalled(x => x.Approve());
        }
        [Test]
        public void ServerFiresClientJoinedEventWhenConnectionStatusBecomesConnected()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);
            bool eventRaised = false;
            serverNetworkSession.ClientJoined += (obj, event_args) => { if (event_args.ID == 100) eventRaised = true; };

            Message msg = serverNetworkSession.ReadMessage();

            Assert.IsNull(msg);
            Assert.IsTrue(eventRaised);
        }
        [Test]
        public void AddsNewConnectionToListOfActiveConnections()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            serverNetworkSession.ReadMessage();

            Assert.AreEqual(stubNetConnection, serverNetworkSession.ActiveConnections[100]);
        }
        [Test]
        public void ServerFiresClientJoinedEventONLYoncePerNewConnection()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);
            int eventRaiseCount = 0;
            serverNetworkSession.ClientJoined += (obj, event_args) => eventRaiseCount++;

            serverNetworkSession.ReadMessage();
            serverNetworkSession.ReadMessage();

            Assert.AreEqual(1, eventRaiseCount);
        }
        [Test]
        public void SendsSuccessfulJoinMessageToClientThatJustJoined()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            serverNetworkSession.ReadMessage();

            stubMessageSender.AssertWasCalled(me => me.SendTo(Arg<Message>.Matches((msg) => (msg.Type == MessageType.SuccessfulJoin) && ((int)msg.Data == 100)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableInOrder1), Arg<INetConnection>.Is.Equal(stubNetConnection)));
        }
        [Test]
        public void SendsNewClientInfoToAllExistingClients()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            serverNetworkSession.ReadMessage();

            stubMessageSender.AssertWasCalled(me => me.SendToAllExcept(Arg<Message>.Matches((msg) => (msg.Type == MessageType.NewPlayer) && ((int)msg.Data == 100)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered), Arg<INetConnection>.Is.Equal(stubNetConnection)));
        }
        [Test]
        public void SendsExistingClientsInfoToNewClient()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);
            var stubCon1 = MockRepository.GenerateStub<INetConnection>();
            var stubCon2 = MockRepository.GenerateStub<INetConnection>();
            stubCon1.Stub(me => me.ConnectionID).Return(20);
            stubCon2.Stub(me => me.ConnectionID).Return(40);
            serverNetworkSession.ActiveConnections.Add(20, stubCon1);
            serverNetworkSession.ActiveConnections.Add(40, stubCon2);

            serverNetworkSession.ReadMessage();

            stubMessageSender.AssertWasNotCalled(me => me.SendTo(Arg<Message>.Matches((msg) => (msg.Type == MessageType.NewPlayer) && ((int)msg.Data == 100)), Arg<NetChannel>.Is.Anything, Arg<INetConnection>.Is.Equal(stubNetConnection)));

            stubMessageSender.AssertWasCalled(me => me.SendTo(Arg<Message>.Matches((msg) => (msg.Type == MessageType.NewPlayer) && ((int)msg.Data == 20)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered), Arg<INetConnection>.Is.Equal(stubNetConnection)));
            stubMessageSender.AssertWasCalled(me => me.SendTo(Arg<Message>.Matches((msg) => (msg.Type == MessageType.NewPlayer) && ((int)msg.Data == 40)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered), Arg<INetConnection>.Is.Equal(stubNetConnection)));
        }

        // **************
        // SENDING TESTS:
        // **************
        [Test]
        public void ServerCanSendMessagesToAllClients()
        {
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };

            serverNetworkSession.SendToAll(msg, NetChannel.Unreliable);

            stubMessageSender.AssertWasCalled(me => me.SendToAll(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Equal(NetChannel.Unreliable)));
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Not a valid player")]
        public void CanOnlySendToValidConnections()
        {
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            stubNetServer.Stub(x => x.Connected).Return(true); // Simply means that *somebody* has joined the server (might not be the connection we are sending to...)
            stubConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connecting);
            serverNetworkSession.ActiveConnections.Add(100, stubConnection);

            serverNetworkSession.SendTo(null, NetChannel.Unreliable, 100);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Not a valid player")]
        public void CantSendToNullConnections()
        {
            stubNetServer.Stub(x => x.Connected).Return(true); // Simply means that *somebody* has joined (might not be the current connection we are sending to...)

            serverNetworkSession.SendTo(null, NetChannel.Unreliable, 100);  // ID 100 not an active connection
        }

        [Test]
        public void ServerCanSendMessageToOneClient()
        {
            serverNetworkSession.ActiveConnections.Add(100, stubNetConnection);
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };

            serverNetworkSession.SendTo(msg, NetChannel.Unreliable, 100);

            stubMessageSender.AssertWasCalled(me => me.SendTo(msg, NetChannel.Unreliable, stubNetConnection));
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Excluded player not connected to network session")]
        public void SendToAllExceptConnectionCantBeNull()
        {

            serverNetworkSession.SendToAllExcept(null, NetChannel.Unreliable, 100); // 100 not an active connection
        }

        [Test]
        public void ServerCanSendMessageToAllExceptOneClient()
        {
            serverNetworkSession.ActiveConnections.Add(100, stubNetConnection);
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };

            serverNetworkSession.SendToAllExcept(msg, NetChannel.Unreliable, 100);

            stubMessageSender.AssertWasCalled(me => me.SendToAllExcept(msg, NetChannel.Unreliable, stubNetConnection));
        }
    }
}
