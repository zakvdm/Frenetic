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
        INetClient stubNetClient;
        LidgrenClientNetworkSession clientNetworkSession;
        [SetUp]
        public void SetUp()
        {
            stubNetClient = MockRepository.GenerateStub<INetClient>();
            clientNetworkSession = new LidgrenClientNetworkSession(stubNetClient, null);
        }

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
            clientNetworkSession = new LidgrenClientNetworkSession(stubNetClient, new XmlMessageSerializer());

            NetBuffer tmpBuffer = new NetBuffer();
            Message msg = new Message() { Type = MessageType.Player, Data = 10 };
            stubNetClient.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);
            stubNetClient.Stub(x => x.Connected).Return(true);

            clientNetworkSession.SendToServer(msg, NetChannel.ReliableUnordered);

            stubNetClient.AssertWasCalled(x => x.CreateBuffer(Arg<int>.Is.Anything));
            stubNetClient.AssertWasCalled(x => x.SendMessage(tmpBuffer, NetChannel.ReliableUnordered));

            byte[] data = (new XmlMessageSerializer()).Serialize(msg);
            Assert.AreEqual(data, tmpBuffer.ReadBytes(data.Length));
        }
    }
}
