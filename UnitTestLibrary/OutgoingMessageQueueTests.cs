using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network;
using Lidgren.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class OutgoingMessageQueueTests
    {
        [Test]
        public void CanConstructWithServerAndClientNetworkSession()
        {
            var stubClientNetworkSession = MockRepository.GenerateStub<IClientNetworkSession>();
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();

            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(stubClientNetworkSession, null);

            Assert.IsNotNull(outgoingMessageQueue);

            outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);

            Assert.IsNotNull(stubServerNetworkSession);
        }

        [Test]
        public void CanWriteAsClientNetworkSession()
        {
            var stubClientNetworkSession = MockRepository.GenerateStub<IClientNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(stubClientNetworkSession, null);
            Message msg = new Message() { Type = MessageType.Event, Data = 2 };

            outgoingMessageQueue.Write(msg, NetChannel.Unreliable);

            stubClientNetworkSession.AssertWasCalled(x => x.SendToServer(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Anything));
        }

        [Test]
        public void WritesUnreliableMessagesFromClientToServer()
        {
            var stubClientNetworkSession = MockRepository.GenerateStub<IClientNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(stubClientNetworkSession, null);
            Message msg = new Message() { Type = MessageType.Event, Data = 2 };

            outgoingMessageQueue.Write(msg);

            stubClientNetworkSession.AssertWasCalled(x => x.SendToServer(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Equal(NetChannel.Unreliable)));
        }

        [Test]
        public void CanWriteAsServerNetworkSession()
        {
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);
            Message msg = new Message() { Type = MessageType.Event, Data = 2 };

            outgoingMessageQueue.Write(msg, NetChannel.Unreliable);

            stubServerNetworkSession.AssertWasCalled(x => x.SendToAll(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Anything));
        }

        [Test]
        public void CanWriteToASpecificClient()
        {
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            Message msg = new Message() { Type = MessageType.ChatLog, Data = 3 };
            Client client = new Client() { LastServerSnap = 1, ID = 20 };
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);

            outgoingMessageQueue.WriteFor(msg, client);

            stubServerNetworkSession.AssertWasCalled(x => x.SendTo(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Equal(NetChannel.Unreliable), Arg<int>.Is.Equal(client.ID)));
        }

        // TODO: REMOVE WHEN EVERYTHING IS UNRELIABLE (no more need for WriteForAllExcept...)
        [Test]
        public void ServerCanWriteMessageForACertainPlayer()
        {
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);
            Message msg = new Message() { Type = MessageType.Event, Data = 3 };

            outgoingMessageQueue.WriteFor(msg, NetChannel.Unreliable, 100);

            stubServerNetworkSession.AssertWasCalled(x => x.SendTo(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Anything, Arg<int>.Is.Equal(100)));
        }

        [Test]
        public void ServerCanWriteMessageForAllExceptACertainPlayer()
        {
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);
            Message msg = new Message() { Type = MessageType.Event, Data = 3 };

            outgoingMessageQueue.WriteForAllExcept(msg, NetChannel.Unreliable, 100);

            stubServerNetworkSession.AssertWasCalled(x => x.SendToAllExcept(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Anything, Arg<int>.Is.Equal(100)));
        }
    }
}
