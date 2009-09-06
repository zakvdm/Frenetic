using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network.Lidgren;
using Lidgren.Network;
using Frenetic.Network;
using Autofac.Builder;
using Frenetic.Engine;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LidgrenServerNetworkSessionTests
    {
        INetServer stubNetServer;
        IServerMessageSender stubMessageSender;
        INetConnection stubNetConnection;
        INetConnection stubDisconnectingConnection;
        LidgrenServerNetworkSession serverNetworkSession;
        [SetUp]
        public void SetUp()
        {
            stubNetServer = MockRepository.GenerateStub<INetServer>();
            stubMessageSender = MockRepository.GenerateStub<IServerMessageSender>();
            stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            stubDisconnectingConnection = MockRepository.GenerateStub<INetConnection>();
            serverNetworkSession = new LidgrenServerNetworkSession(stubNetServer, stubMessageSender, DummyLogger.Factory);

            stubNetConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connected);
            stubNetConnection.Stub(x => x.ConnectionID).Return(100);
            stubDisconnectingConnection.Stub(x => x.Status).Return(NetConnectionStatus.Disconnecting);
            stubDisconnectingConnection.Stub(x => x.ConnectionID).Return(200);

            serverNetworkSession.ActiveConnections.Add(200, stubDisconnectingConnection);
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
            tmpBuffer.Write(new Message() { Items = { new Item() { Type = ItemType.NewClient, Data = 10 } } });
            stubNetServer.Stub(x => x.CreateBuffer()).Return(tmpBuffer);
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                            out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy,
                            out Arg<INetConnection>.Out(stubNetConnection).Dummy))
                            .Return(true);

            Message msg = serverNetworkSession.ReadNextMessage();

            stubNetServer.AssertWasCalled(x => x.ReadMessage(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                        out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy,
                                        out Arg<INetConnection>.Out(stubNetConnection).Dummy));
            Assert.AreEqual(ItemType.NewClient, msg.Items[0].Type);
            Assert.AreEqual(10, msg.Items[0].Data);
        }

        // NEW CLIENT HANDLING:
        [Test]
        public void ServerApprovesNewConnection()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.ConnectionApproval).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            Message msg = serverNetworkSession.ReadNextMessage();

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

            Message msg = serverNetworkSession.ReadNextMessage();

            Assert.IsNull(msg);
            Assert.IsTrue(eventRaised);
        }
        [Test]
        public void AddsNewConnectionToListOfActiveConnections()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            serverNetworkSession.ReadNextMessage();

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

            serverNetworkSession.ReadNextMessage();
            serverNetworkSession.ReadNextMessage();

            Assert.AreEqual(1, eventRaiseCount);
        }
        [Test]
        public void SendsSuccessfulJoinMessageToClientThatJustJoined()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            serverNetworkSession.ReadNextMessage();

            stubMessageSender.AssertWasCalled(me => me.SendTo(Arg<Message>.Matches((msg) => (msg.Items[0].Type == ItemType.SuccessfulJoin) && ((int)msg.Items[0].Data == 100)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableInOrder1), Arg<INetConnection>.Is.Equal(stubNetConnection)));
        }
        [Test]
        public void SendsNewClientInfoToAllExistingClients()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubNetConnection).Dummy)).Return(true);

            serverNetworkSession.ReadNextMessage();

            stubMessageSender.AssertWasCalled(me => me.SendToAllExcept(Arg<Message>.Matches((msg) => (msg.Items[0].Type == ItemType.NewClient) && ((int)msg.Items[0].Data == 100)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered), Arg<INetConnection>.Is.Equal(stubNetConnection)));
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

            serverNetworkSession.ReadNextMessage();

            stubMessageSender.AssertWasNotCalled(me => me.SendTo(Arg<Message>.Matches((msg) => (msg.Items[0].Type == ItemType.NewClient) && ((int)msg.Items[0].Data == 100)), Arg<NetChannel>.Is.Anything, Arg<INetConnection>.Is.Equal(stubNetConnection)));

            stubMessageSender.AssertWasCalled(me => me.SendTo(Arg<Message>.Matches((msg) => (msg.Items[0].Type == ItemType.NewClient) && ((int)msg.Items[0].Data == 20)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered), Arg<INetConnection>.Is.Equal(stubNetConnection)));
            stubMessageSender.AssertWasCalled(me => me.SendTo(Arg<Message>.Matches((msg) => (msg.Items[0].Type == ItemType.NewClient) && ((int)msg.Items[0].Data == 40)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered), Arg<INetConnection>.Is.Equal(stubNetConnection)));
        }

        // DISCONNECTING CLIENT:
        [Test]
        public void ServerFiresClientDisconnectedEventWhenConnectionStatusBecomesDisconnected()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubDisconnectingConnection).Dummy)).Return(true);
            bool eventRaised = false;
            serverNetworkSession.ClientDisconnected += (obj, event_args) => { if (event_args.ID == 200) eventRaised = true; };

            Message msg = serverNetworkSession.ReadNextMessage();

            Assert.IsNull(msg);
            Assert.IsTrue(eventRaised);
        }
        [Test]
        public void RemovesConnectionFromListOfActiveConnections()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubDisconnectingConnection).Dummy)).Return(true);

            serverNetworkSession.ReadNextMessage();

            Assert.IsFalse(serverNetworkSession.ActiveConnections.ContainsValue(stubDisconnectingConnection));
        }
        [Test]
        public void ServerFiresClientDisconnectedEventONLYoncePerDisconnectingConnection()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubDisconnectingConnection).Dummy)).Return(true);
            int eventRaiseCount = 0;
            serverNetworkSession.ClientDisconnected += (obj, event_args) => eventRaiseCount++;

            serverNetworkSession.ReadNextMessage();
            serverNetworkSession.ReadNextMessage();

            Assert.AreEqual(1, eventRaiseCount);
        }
        [Test]
        public void SendsDisconnectingClientInfoToAllExistingClients()
        {
            stubNetServer.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Anything,
                    out Arg<NetMessageType>.Out(NetMessageType.StatusChanged).Dummy,
                    out Arg<INetConnection>.Out(stubDisconnectingConnection).Dummy)).Return(true);

            serverNetworkSession.ReadNextMessage();

            stubMessageSender.AssertWasCalled(me => me.SendToAll(Arg<Message>.Matches((msg) => (msg.Items[0].Type == ItemType.DisconnectingClient) && ((int)msg.Items[0].Data == 200)), Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered))); 
        }

        // **************
        // SENDING TESTS:
        // **************
        [Test]
        public void SendIsForwardedCorrectly()
        {
            Message msg = new Message();

            serverNetworkSession.Send(msg, NetChannel.UnreliableInOrder11);

            stubMessageSender.AssertWasCalled(me => me.SendToAll(msg, NetChannel.UnreliableInOrder11));
        }

        [Test]
        public void ServerCanSendMessagesToAllClients()
        {
            Message msg = new Message() { Items = { new Item() { Data = new byte[] { 1, 2, 3, 4 } } } };

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
            Message msg = new Message() { Items = { new Item() { Data = new byte[] { 1, 2, 3, 4 } } } };

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
            Message msg = new Message() { Items = { new Item() { Data = new byte[] { 1, 2, 3, 4 } } } };

            serverNetworkSession.SendToAllExcept(msg, NetChannel.Unreliable, 100);

            stubMessageSender.AssertWasCalled(me => me.SendToAllExcept(msg, NetChannel.Unreliable, stubNetConnection));
        }
    }
}
