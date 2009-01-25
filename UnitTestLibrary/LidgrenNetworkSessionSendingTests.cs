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
    public class LidgrenNetworkSessionSendingTests
    {
        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Server can't send to server!")]
        public void OnlyClientCanSendToServer()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            LidgrenNetworkSession serverNS = new LidgrenNetworkSession(stubNetServer);

            serverNS.SendToServer(null, NetChannel.Unreliable);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                            ExpectedMessage = "Client not connected to server")]
        public void ClientCantSendToServerWhenNotConnected()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            LidgrenNetworkSession clientNS = new LidgrenNetworkSession(stubNetClient);

            clientNS.SendToServer(new Message(), NetChannel.Unreliable);
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

            clientNetworkSession.SendToServer(msg, NetChannel.ReliableUnordered);

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
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Client can only send to Server")]
        public void OnlyServerCanSendToConnection()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenNetworkSession clientNS = new LidgrenNetworkSession(stubNetClient);

            clientNS.SendTo(null, NetChannel.Unreliable, stubConnection);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Not a valid Connection")]
        public void CanOnlySendToValidConnections()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            stubNetServer.Stub(x => x.Connected).Return(true); // Simply means that *somebody* has joined (might not be the current connection we are sending to...)
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            stubConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connecting);
            LidgrenNetworkSession serverNS = new LidgrenNetworkSession(stubNetServer);

            serverNS.SendTo(null, NetChannel.Unreliable, stubConnection);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Not a valid Connection")]
        public void CantSendToNullConnections()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            stubNetServer.Stub(x => x.Connected).Return(true); // Simply means that *somebody* has joined (might not be the current connection we are sending to...)
            LidgrenNetworkSession serverNS = new LidgrenNetworkSession(stubNetServer);

            serverNS.SendTo(null, NetChannel.Unreliable, null);
        }

        [Test]
        public void ServerCanSendMessageToOneClient()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            stubConnection.Stub(x => x.Status).Return(NetConnectionStatus.Connected);
            LidgrenNetworkSession serverNS = new LidgrenNetworkSession(stubNetServer);
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);

            serverNS.SendTo(msg, NetChannel.Unreliable, stubConnection);

            stubNetServer.AssertWasCalled(x => x.SendMessage(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Anything, Arg<INetConnection>.Is.Equal(stubConnection)));
            Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, (new XmlMessageSerializer()).Deserialize(tmpBuffer.ToArray()).Data);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Client can only send to the Server")]
        public void OnlyServerCanSendToAllExcept()
        {
            var stubNetClient = MockRepository.GenerateStub<INetClient>();
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenNetworkSession clientNS = new LidgrenNetworkSession(stubNetClient);

            clientNS.SendToAllExcept(null, NetChannel.Unreliable, stubConnection);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(System.InvalidOperationException),
                    ExpectedMessage = "Excluded connection can't be null")]
        public void SendToAllExceptConnectionCantBeNull()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            LidgrenNetworkSession serverNS = new LidgrenNetworkSession(stubNetServer);

            serverNS.SendToAllExcept(null, NetChannel.Unreliable, null);
        }

        [Test]
        public void ServerCanSendMessageToAllExceptOneClient()
        {
            var stubNetServer = MockRepository.GenerateStub<INetServer>();
            var stubConnection = MockRepository.GenerateStub<INetConnection>();
            LidgrenNetworkSession serverNS = new LidgrenNetworkSession(stubNetServer);
            Message msg = new Message() { Data = new byte[] { 1, 2, 3, 4 } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer(Arg<int>.Is.Anything)).Return(tmpBuffer);

            serverNS.SendToAllExcept(msg, NetChannel.Unreliable, stubConnection);

            stubNetServer.AssertWasCalled(x => x.SendToAll(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Anything, Arg<INetConnection>.Is.Equal(stubConnection)));
            Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, (new XmlMessageSerializer()).Deserialize(tmpBuffer.ToArray()).Data);
        }
    }
}
