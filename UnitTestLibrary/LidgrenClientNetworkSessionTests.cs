using System;
using NUnit.Framework;
using Frenetic.Network.Lidgren;
using Rhino.Mocks;
using Lidgren.Network;
using Frenetic.Network;


namespace UnitTestLibrary
{
    [TestFixture]
    public class LidgrenClientNetworkSessionTests
    {
        IMessageSerializer stubSerializer;
        INetClient stubNetClient;
        LidgrenClientNetworkSession clientNetworkSession;
        [SetUp]
        public void SetUp()
        {
            stubNetClient = MockRepository.GenerateStub<INetClient>();
            stubSerializer = MockRepository.GenerateStub<IMessageSerializer>();
            clientNetworkSession = new LidgrenClientNetworkSession(stubNetClient, stubSerializer, MockRepository.GenerateStub<log4net.ILog>());
        }

        // CONNECTING:
        [Test]
        public void ClientCanJoinSessionCorrectlyWithIP()
        {
            string ip = "ip";
            int port = 1;

            clientNetworkSession.Join(ip, port);

            stubNetClient.AssertWasCalled(x => x.Start());
            stubNetClient.AssertWasCalled(x => x.Connect(ip, port));
        }

        [Test]
        public void ClientCanSearchForLocalSessions()
        {
            int port = 1;

            clientNetworkSession.Join(port);

            stubNetClient.AssertWasCalled(x => x.Start());
            stubNetClient.AssertWasCalled(x => x.DiscoverLocalServers(port));
        }

        [Test]
        public void ClientConnectsToFoundServer()
        {
            NetBuffer tmp = new NetBuffer();

            stubNetClient.Stub(x => x.CreateBuffer()).Return(tmp);
            stubNetClient.Stub(x => x.ReadMessage(Arg<NetBuffer>.Is.Equal(tmp),
                                            out Arg<NetMessageType>.Out(NetMessageType.ServerDiscovered).Dummy))
                                            .Return(true);
            stubNetClient.Stub(x => x.Connect(Arg<System.Net.IPEndPoint>.Is.Anything, Arg<byte[]>.Is.Anything));

            clientNetworkSession.ReadMessage();

            stubNetClient.AssertWasCalled(x => x.Connect(Arg<System.Net.IPEndPoint>.Is.Anything, Arg<byte[]>.Is.Anything));
        }

        // RECEIVING DATA:
        [Test]
        public void RaisesClientJoinedEventForLocalPlayerWhenNotifiedOfSuccessfulJoin()
        {
            NetBuffer buffer = new NetBuffer();
            stubNetClient.Stub(me => me.CreateBuffer()).Return(buffer);
            stubNetClient.Stub(me => me.ReadMessage(Arg<NetBuffer>.Is.Equal(buffer), out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy)).Return(true);
            stubSerializer.Stub(me => me.Deserialize(buffer.ReadBytes(buffer.LengthBytes))).Return(new Message() { Type = MessageType.SuccessfulJoin, Data = 100 });
            bool raisedEvent = false;
            clientNetworkSession.ClientJoined += (obj, args) => { if ((args.ID == 100) && (args.IsLocalClient)) raisedEvent = true; };

            clientNetworkSession.ReadMessage();

            Assert.IsTrue(raisedEvent);
        }
        [Test]
        public void RaisesClientJoinedEventForNetworkPlayersWhenNotifiedOfNewPlayers()
        {
            NetBuffer buffer = new NetBuffer();
            stubNetClient.Stub(me => me.CreateBuffer()).Return(buffer);
            stubNetClient.Stub(me => me.ReadMessage(Arg<NetBuffer>.Is.Equal(buffer), out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy)).Return(true);
            stubSerializer.Stub(me => me.Deserialize(buffer.ReadBytes(buffer.LengthBytes))).Return(new Message() { Type = MessageType.NewClient, Data = 100 });
            bool raisedEvent = false;
            clientNetworkSession.ClientJoined += (obj, args) => { if ((args.ID == 100) && (!args.IsLocalClient)) raisedEvent = true; };

            clientNetworkSession.ReadMessage();

            Assert.IsTrue(raisedEvent);
        }
        [Test]
        public void RaisesClientDisconnectedEventCorrectlyForDisconnectingClients()
        {
            stubNetClient.Stub(me => me.CreateBuffer()).Return(new NetBuffer());
            stubNetClient.Stub(me => me.ReadMessage(Arg<NetBuffer>.Is.Anything, out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy)).Return(true);
            stubSerializer.Stub(me => me.Deserialize(Arg<byte[]>.Is.Anything)).Return(new Message() { Type = MessageType.DisconnectingClient, Data = 100 });
            bool raisedEvent = false;
            clientNetworkSession.ClientDisconnected += (obj, args) => { if ((args.ID == 100) && (!args.IsLocalClient)) raisedEvent = true; };

            clientNetworkSession.ReadMessage();

            Assert.IsTrue(raisedEvent);
        }

        // SENDING DATA:
        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                            ExpectedMessage = "Client not connected to server")]
        public void ClientCantSendToServerWhenNotConnected()
        {
            clientNetworkSession.SendToServer(new Message(), NetChannel.Unreliable);
        }

        [Test]
        public void ClientProcessesAndSendsToServer()
        {
            NetBuffer tmpBuffer = new NetBuffer();
            Message msg = new Message() { Type = MessageType.Player, Data = 10 };
            stubNetClient.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);
            stubNetClient.Stub(x => x.Status).Return(NetConnectionStatus.Connected);
            byte[] data = { 0, 2 };
            stubSerializer.Stub(x => x.Serialize(msg)).Return(data);

            clientNetworkSession.SendToServer(msg, NetChannel.ReliableUnordered);

            stubNetClient.AssertWasCalled(x => x.CreateBuffer(Arg<int>.Is.Anything));
            stubNetClient.AssertWasCalled(x => x.SendMessage(tmpBuffer, NetChannel.ReliableUnordered));
            Assert.AreEqual(data, tmpBuffer.ReadBytes(data.Length));
        }
    }
}
