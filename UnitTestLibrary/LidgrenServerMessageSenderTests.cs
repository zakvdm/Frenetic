using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Network;
using Lidgren.Network;
using Frenetic.Network.Lidgren;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LidgrenServerMessageSenderTests
    {
        LidgrenServerMessageSender serverMessageSender;
        INetServer stubNetServer;
        INetConnection stubNetConnection;

        [SetUp]
        public void SetUp()
        {
            stubNetServer = MockRepository.GenerateStub<INetServer>();
            stubNetConnection = MockRepository.GenerateStub<INetConnection>();
            serverMessageSender = new LidgrenServerMessageSender(stubNetServer, DummyLogger.Factory);
        }

        [Test]
        public void ServerCanSendMessagesToAllClients()
        {
            Message msg = new Message() { Items = { new Item() { Type = ItemType.DisconnectingClient, Data = 999 } } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer()).Return(tmpBuffer);

            serverMessageSender.SendToAll(msg, NetChannel.Unreliable);

            stubNetServer.AssertWasCalled(x => x.SendToAll(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Equal(NetChannel.Unreliable)));
            Assert.AreEqual(999, tmpBuffer.ReadMessage().Items[0].Data);
        }

        [Test]
        public void ServerCanSendMessageToOneClient()
        {
            Message msg = new Message() { Items = { new Item() { Type = ItemType.NewClient, Data = 333 } } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer()).Return(tmpBuffer);

            serverMessageSender.SendTo(msg, NetChannel.Unreliable, stubNetConnection);

            stubNetServer.AssertWasCalled(x => x.SendMessage(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Equal(NetChannel.Unreliable), Arg<INetConnection>.Is.Equal(stubNetConnection)));
            Assert.AreEqual(333, tmpBuffer.ReadMessage().Items[0].Data);
        }

        [Test]
        public void ServerCanSendMessageToAllExceptOneClient()
        {
            Message msg = new Message() { Items = { new Item() { Type = ItemType.SuccessfulJoin, Data = 666 } } };
            NetBuffer tmpBuffer = new NetBuffer();

            stubNetServer.Stub(x => x.CreateBuffer()).Return(tmpBuffer);

            serverMessageSender.SendToAllExcept(msg, NetChannel.Unreliable, stubNetConnection);

            stubNetServer.AssertWasCalled(x => x.SendToAll(Arg<NetBuffer>.Is.Equal(tmpBuffer),
                                                        Arg<NetChannel>.Is.Equal(NetChannel.Unreliable), Arg<INetConnection>.Is.Equal(stubNetConnection)));
            Assert.AreEqual(666, tmpBuffer.ReadMessage().Items[0].Data);
        }
    }
}
