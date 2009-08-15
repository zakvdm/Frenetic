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

            stubNetworkSession.AssertWasCalled(me => me.Send(Arg<Message>.Matches((msg) => msg.Items[0] == item1 && msg.Items[1] == item2), Arg<NetChannel>.Is.Equal(NetChannel.UnreliableInOrder1)));
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
        public void SendsReliableMessagesImmediately()
        {
            var item = new Item() { Data = 143 };
            msgQueue.AddToReliableQueue(item);

            stubNetworkSession.AssertWasCalled(me => me.Send(Arg<Message>.Matches((msg) => msg.Items[0] == item), Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered)));
        }
    }
}
