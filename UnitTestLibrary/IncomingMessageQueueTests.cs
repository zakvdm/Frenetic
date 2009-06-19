using System;
using System.Collections.Generic;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using System.Xml.Serialization;
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
            client = new Client(null, null) { ID = 100 };
            stubClientStateTracker.Stub(me => me.FindNetworkClient(100)).Return(client);

            mq = new IncomingMessageQueue(stubNS, stubClientStateTracker);
        }

        [Test]
        public void EmptyQueueReturnsFalseOnHasAvailableCall()
        {
            stubNS.Stub(x => x.ReadMessage()).Return(null);

            Assert.IsFalse(mq.HasAvailable(MessageType.Player));
        }

        [Test]
        public void HasAvailableTrueWhenMessageAvailable()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, ClientID = 100 });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.IsTrue(mq.HasAvailable(MessageType.Player));
        }
        
        [Test]
        public void ReturnsMessageWhenAvailable()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[3] { 1, 2, 3 }, ClientID = 100 });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadMessage(MessageType.Player).Data);
        }

        [Test]
        public void ReadMessageReturnsMessageObject()
        {
            Message msg = new Message() { Type = MessageType.SuccessfulJoin, ClientID = 100, Data = 1 };
            queueMH.QueuedMessages.Enqueue(msg);
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(msg, mq.ReadMessage(MessageType.SuccessfulJoin));
        }

        [Test]
        public void CanQueueMoreThanOneMessage()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[3] { 1, 2, 3 }, ClientID = 100 });
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[2] { 4, 5 }, ClientID = 100 });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadMessage(MessageType.Player).Data);
            Assert.AreEqual(new byte[2] { 4, 5 }, mq.ReadMessage(MessageType.Player).Data);
        }

        [Test]
        public void StoresSeperateQueuesForDifferentMessageTypes()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[3] { 1, 2, 3 }, ClientID = 100 });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.IsFalse(mq.HasAvailable(MessageType.PlayerSettings));
        }

        // INVALID MESSAGES:
        [Test]
        public void HasAvailableDiscardsInvalidMessages()
        {
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = 1, ClientID = 200 });
            stubNS.Stub(me => me.ReadMessage()).Do(queueMH.GetNextQueuedMessage);
            stubClientStateTracker.Stub(me => me.FindNetworkClient(200)).Return(null);

            Assert.IsFalse(mq.HasAvailable(MessageType.Player));
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