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
        public void CanWriteAsServerNetworkSession()
        {
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);
            Message msg = new Message() { Type = MessageType.Event, Data = 2 };

            outgoingMessageQueue.Write(msg, NetChannel.Unreliable);

            stubServerNetworkSession.AssertWasCalled(x => x.SendToAll(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Anything));
        }

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
