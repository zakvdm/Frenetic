using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network;
using Lidgren.Network;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class OutgoingMessageQueueTests
    {
        OutgoingMessageQueue msgQueue;
        INetworkSession stubNetworkSession;
        [SetUp]
        public void SetUp()
        {
            stubNetworkSession = MockRepository.GenerateStub<INetworkSession>();
            msgQueue = new OutgoingMessageQueue(stubNetworkSession);
        }

        [Test]
        public void ConstructsMessageWithAllQueuedObjects()
        {
            Item item1 = new Item() { Data = "a" };
            Item item2 = new Item() { Data = 3 };

            msgQueue.AddToQueue(item1);
            msgQueue.AddToQueue(item2);
            msgQueue.SendMessagesOnQueue();

            stubNetworkSession.AssertWasCalled(me => me.Send(Arg<Message>.Matches((msg) => msg.Items[0] == item1 && msg.Items[1] == item2), Arg<NetChannel>.Is.Equal(NetChannel.Unreliable)));
        }

        [Test]
        public void ClearsTheMessageWhenYouSend()
        {
            msgQueue.AddToQueue(new Item() { Data = 323 });
            Assert.AreEqual(1, msgQueue.CurrentMessage.Items.Count);

            msgQueue.SendMessagesOnQueue();

            Assert.AreEqual(0, msgQueue.CurrentMessage.Items.Count);
        }

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
            Message msg = new Message() { Items = { new Item() { Type = ItemType.Event, Data = 2 } } };

            outgoingMessageQueue.Write(msg, NetChannel.Unreliable);

            stubClientNetworkSession.AssertWasCalled(x => x.Send(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Anything));
        }

        [Test]
        public void WritesUnreliableMessagesFromClientToServer()
        {
            var stubClientNetworkSession = MockRepository.GenerateStub<IClientNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(stubClientNetworkSession, null);
            Message msg = new Message() { Items = { new Item() { Type = ItemType.Event, Data = 2 } } };

            outgoingMessageQueue.Write(msg);

            stubClientNetworkSession.AssertWasCalled(x => x.Send(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Equal(NetChannel.UnreliableInOrder1)));
        }

        [Test]
        public void CanWriteAsServerNetworkSession()
        {
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);
            Message msg = new Message() { Items = { new Item() { Type = ItemType.Event, Data = 2 } } };

            outgoingMessageQueue.Write(msg, NetChannel.Unreliable);

            stubServerNetworkSession.AssertWasCalled(x => x.SendToAll(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Anything));
        }

        [Test]
        public void CanWriteToASpecificClient()
        {
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            Message msg = new Message() { Items = { new Item() { Type = ItemType.ChatLog, Data = 3 } } };
            Client client = new Client(null) { LastServerSnap = 1, ID = 20 };
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);

            outgoingMessageQueue.WriteFor(msg, client);

            stubServerNetworkSession.AssertWasCalled(x => x.SendTo(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Equal(NetChannel.Unreliable), Arg<int>.Is.Equal(client.ID)));
        }

        [Test]
        public void ServerCanWriteMessageForACertainPlayer()
        {
            var stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            OutgoingMessageQueue outgoingMessageQueue = new OutgoingMessageQueue(null, stubServerNetworkSession);
            Message msg = new Message() { Items = { new Item() { Type = ItemType.Event, Data = 3 } } };

            outgoingMessageQueue.WriteFor(msg, NetChannel.Unreliable, 100);

            stubServerNetworkSession.AssertWasCalled(x => x.SendTo(Arg<Message>.Is.Equal(msg), Arg<NetChannel>.Is.Anything, Arg<int>.Is.Equal(100)));
        }
    }
}
