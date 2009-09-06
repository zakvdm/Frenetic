using System;
using System.Collections.Generic;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using System.IO;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class IncomingMessageQueueTests
    {
        QueuedMessageHelper<Message> queueMH;
        INetworkSession stubNS;
        IClientStateTracker stubClientStateTracker;
        Client client;
        IncomingMessageQueue mq;
        [SetUp]
        public void SetUp()
        {
            queueMH = new QueuedMessageHelper<Message>();
            stubNS = MockRepository.GenerateStub<INetworkSession>();
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            client = new Client(null) { ID = 100 };
            stubClientStateTracker.Stub(me => me.FindNetworkClient(100)).Return(client);

            mq = new IncomingMessageQueue(stubNS, stubClientStateTracker);
        }

        [Test]
        public void EmptyQueueReturnsFalseOnHasAvailableCall()
        {
            stubNS.Stub(x => x.ReadNextMessage()).Return(null);

            Assert.IsFalse(mq.HasAvailable(ItemType.Player));
        }

        [Test]
        public void HasAvailableTrueWhenMessageAvailable()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { new Item() { Type = ItemType.Player, ClientID = 100 } } });
            stubNS.Stub(x => x.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.IsTrue(mq.HasAvailable(ItemType.Player));
        }
        
        [Test]
        public void ReturnsMessageWhenAvailable()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { new Item() { Type = ItemType.Player, Data = new byte[3] { 1, 2, 3 }, ClientID = 100 } } });
            stubNS.Stub(x => x.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadItem(ItemType.Player).Data);
        }

        [Test]
        public void ReadItemReturnsItemObject()
        {
            Item item = new Item() { Type = ItemType.SuccessfulJoin, ClientID = 100, Data = 1 };
            Message msg = new Message() { Items = { item } };
            queueMH.QueuedMessages.Enqueue(msg);
            stubNS.Stub(x => x.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(item, mq.ReadItem(ItemType.SuccessfulJoin));
        }

        [Test]
        public void ReadsAllItemsInMessage()
        {
            Item item1 = new Item() { Type = ItemType.Player, Data = 3 };
            Item item2 = new Item() { Type = ItemType.NewClient, Data = 20 };
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { item1, item2 } });
            stubNS.Stub(me => me.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.IsTrue(mq.HasAvailable(ItemType.Player));
            Assert.IsTrue(mq.HasAvailable(ItemType.NewClient));
            Assert.AreEqual(item2, mq.ReadItem(ItemType.NewClient));
            Assert.AreEqual(item1, mq.ReadItem(ItemType.Player));
        }

        [Test]
        public void CanQueueMoreThanOneMessage()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { new Item() { Type = ItemType.Player, Data = new byte[3] { 1, 2, 3 }, ClientID = 100 } } });
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { new Item() { Type = ItemType.Player, Data = new byte[2] { 4, 5 }, ClientID = 100 } } });
            stubNS.Stub(x => x.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadItem(ItemType.Player).Data);
            Assert.AreEqual(new byte[2] { 4, 5 }, mq.ReadItem(ItemType.Player).Data);
        }

        [Test]
        public void StoresSeperateQueuesForDifferentTypes()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { new Item() { Type = ItemType.Player, Data = new byte[3] { 1, 2, 3 }, ClientID = 100 } } });
            stubNS.Stub(x => x.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.IsFalse(mq.HasAvailable(ItemType.PlayerSettings));
        }

        [Test]
        public void DoesntDiscardItemsAboutTheLocalClient()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { new Item() { Type = ItemType.PlayerSettings, ClientID = 300, Data = 4 } } });
            stubNS.Stub(me => me.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);
            stubClientStateTracker.Stub(me => me.LocalClient).Return(new LocalClient(null) { ID = 300 });

            Assert.IsTrue(mq.HasAvailable(ItemType.PlayerSettings));
        }
        [Test]
        public void HasAvailableDiscardsInvalidItems()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { new Item() { Type = ItemType.Player, Data = 1, ClientID = 200 } } });
            stubNS.Stub(me => me.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);
            stubClientStateTracker.Stub(me => me.FindNetworkClient(200)).Return(null);

            Assert.IsFalse(mq.HasAvailable(ItemType.Player));
        }
        [Test]
        public void DoesntDiscardItemsThatDontCareAboutHavingAValidClient()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Items = { new Item() { Type = ItemType.ChatLog, Data = 1 } } }); // No ClientID set, so the ClientID's not relevant
            stubNS.Stub(me => me.ReadNextMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.IsTrue(mq.HasAvailable(ItemType.ChatLog));
        }
    }

    public class QueuedMessageHelper<T>// where T:class
    {
        public QueuedMessageHelper()
        {
            QueuedMessages = new Queue<T>();
        }
        public Queue<T> QueuedMessages { get; private set; }
        public DoDelegate GetNextQueuedMessage
        {
            get
            {
                return new DoDelegate(getNextQueuedMessage);
            }
        }
        public delegate T DoDelegate();
        private T getNextQueuedMessage()
        {
            if (QueuedMessages.Count > 0)
            {
                return QueuedMessages.Dequeue();
            }
            //return null;
            return default(T);
        }
    }

    public class QueuedMessageHelper<T, Arg> where T : class
    {
        public QueuedMessageHelper()
        {
            QueuedMessages = new Queue<T>();
        }
        public Queue<T> QueuedMessages { get; private set; }
        public DoDelegate GetNextQueuedMessage
        {
            get
            {
                return new DoDelegate(getNextQueuedMessage);
            }
        }
        public HasAvailableDelegate HasMessageAvailable
        {
            get
            {
                return new HasAvailableDelegate(hasMessageAvailable);
            }
        }
        public delegate T DoDelegate(Arg arg);
        public delegate bool HasAvailableDelegate(Arg arg);
        private T getNextQueuedMessage(Arg arg)
        {
            if (QueuedMessages.Count > 0)
            {
                return QueuedMessages.Dequeue();
            }
            return null;
        }
        private bool hasMessageAvailable(Arg arg)
        {
            return (QueuedMessages.Count > 0);
        }
    }
}