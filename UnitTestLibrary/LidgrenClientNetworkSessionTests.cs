using System;
using NUnit.Framework;
using Frenetic.Network.Lidgren;
using Rhino.Mocks;
using Lidgren.Network;
using Frenetic.Network;
using Frenetic.Engine;


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
            clientNetworkSession = new LidgrenClientNetworkSession(stubNetClient, stubSerializer, DummyLogger.Factory);
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

            clientNetworkSession.ReadNextMessage();

            stubNetClient.AssertWasCalled(x => x.Connect(Arg<System.Net.IPEndPoint>.Is.Anything, Arg<byte[]>.Is.Anything));
        }

        // RECEIVING DATA:
        [Test]
        public void HandlesAllItemsOnTheMessage()
        {
            var msg = new Message() { Items = { new Item() { Type = ItemType.SuccessfulJoin, Data = 143 }, new Item() { Type = ItemType.NewClient, Data = 3 }, new Item() { Type = ItemType.Player, Data = 11 } } };
            NetBuffer buffer = new NetBuffer();
            stubNetClient.Stub(me => me.CreateBuffer()).Return(buffer);
            stubNetClient.Stub(me => me.ReadMessage(Arg<NetBuffer>.Is.Equal(buffer), out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy)).Return(true);
            stubSerializer.Stub(me => me.Deserialize(buffer.ReadBytes(buffer.LengthBytes))).Return(msg);
            clientNetworkSession.ClientJoined += delegate { };

            var unprocessedMsg = clientNetworkSession.ReadNextMessage();

            Assert.AreEqual(1, unprocessedMsg.Items.Count);
            Assert.AreEqual(ItemType.Player, unprocessedMsg.Items[0].Type);
        }

        [Test]
        public void RaisesClientJoinedEventForLocalPlayerWhenNotifiedOfSuccessfulJoin()
        {
            NetBuffer buffer = new NetBuffer();
            stubNetClient.Stub(me => me.CreateBuffer()).Return(buffer);
            stubNetClient.Stub(me => me.ReadMessage(Arg<NetBuffer>.Is.Equal(buffer), out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy)).Return(true);
            stubSerializer.Stub(me => me.Deserialize(buffer.ReadBytes(buffer.LengthBytes))).Return(new Message() { Items = { new Item() { Type = ItemType.SuccessfulJoin, Data = 100 } } });
            bool raisedEvent = false;
            clientNetworkSession.ClientJoined += (obj, args) => { if ((args.ID == 100) && (args.IsLocalClient)) raisedEvent = true; };

            clientNetworkSession.ReadNextMessage();

            Assert.IsTrue(raisedEvent);
        }
        [Test]
        public void RaisesClientJoinedEventForNetworkPlayersWhenNotifiedOfNewPlayers()
        {
            NetBuffer buffer = new NetBuffer();
            stubNetClient.Stub(me => me.CreateBuffer()).Return(buffer);
            stubNetClient.Stub(me => me.ReadMessage(Arg<NetBuffer>.Is.Equal(buffer), out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy)).Return(true);
            stubSerializer.Stub(me => me.Deserialize(buffer.ReadBytes(buffer.LengthBytes))).Return(new Message() { Items = { new Item() { Type = ItemType.NewClient, Data = 100 } } });
            bool raisedEvent = false;
            clientNetworkSession.ClientJoined += (obj, args) => { if ((args.ID == 100) && (!args.IsLocalClient)) raisedEvent = true; };

            clientNetworkSession.ReadNextMessage();

            Assert.IsTrue(raisedEvent);
        }
        [Test]
        public void RaisesClientDisconnectedEventCorrectlyForDisconnectingClients()
        {
            stubNetClient.Stub(me => me.CreateBuffer()).Return(new NetBuffer());
            stubNetClient.Stub(me => me.ReadMessage(Arg<NetBuffer>.Is.Anything, out Arg<NetMessageType>.Out(NetMessageType.Data).Dummy)).Return(true);
            stubSerializer.Stub(me => me.Deserialize(Arg<byte[]>.Is.Anything)).Return(new Message() { Items = { new Item() { Type = ItemType.DisconnectingClient, Data = 100 } } });
            bool raisedEvent = false;
            clientNetworkSession.ClientDisconnected += (obj, args) => { if ((args.ID == 100) && (!args.IsLocalClient)) raisedEvent = true; };

            clientNetworkSession.ReadNextMessage();

            Assert.IsTrue(raisedEvent);
        }

        // SENDING DATA:
        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                            ExpectedMessage = "Client not connected to server")]
        public void ClientCantSendToServerWhenNotConnected()
        {
            clientNetworkSession.Send(new Message(), NetChannel.Unreliable);
        }

        [Test]
        public void ClientProcessesAndSendsToServer()
        {
            NetBuffer tmpBuffer = new NetBuffer();
            Message msg = new Message() { Items = { new Item() { Type = ItemType.Player, Data = 10 } } };
            stubNetClient.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);
            stubNetClient.Stub(x => x.Status).Return(NetConnectionStatus.Connected);
            byte[] data = { 0, 2 };
            stubSerializer.Stub(x => x.Serialize(msg)).Return(data);

            clientNetworkSession.Send(msg, NetChannel.ReliableUnordered);

            stubNetClient.AssertWasCalled(x => x.CreateBuffer(Arg<int>.Is.Anything));
            stubNetClient.AssertWasCalled(x => x.SendMessage(tmpBuffer, NetChannel.ReliableUnordered));
            Assert.AreEqual(data, tmpBuffer.ReadBytes(data.Length));
        }
    }
}
