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
        [SetUp]
        public void SetUp()
        {
            queueMH = new QueuedMessageHelper<Message>();
        }

        [Test]
        public void CanConstruct()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            IncomingMessageQueue mq = new IncomingMessageQueue(stubNS);

            Assert.IsNotNull(mq);
        }

        [Test]
        public void EmptyQueueReturnsFalseOnMessageAvailable()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            IncomingMessageQueue mq = new IncomingMessageQueue(stubNS);
            stubNS.Stub(x => x.ReadMessage()).Return(null);

            Assert.IsNull(mq.ReadWholeMessage(MessageType.Player));
        }

        [Test]
        public void ReturnsMessageWhenAvailable()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            IncomingMessageQueue mq = new IncomingMessageQueue(stubNS);
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[3] { 1, 2, 3 } });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadWholeMessage(MessageType.Player).Data);
        }

        [Test]
        public void ReadWholeMessageReturnsMessageObject()
        {
            var stubNetworkSession = MockRepository.GenerateStub<INetworkSession>();
            IncomingMessageQueue incomingMessageQueue = new IncomingMessageQueue(stubNetworkSession);
            Message msg = new Message() { Type = MessageType.SuccessfulJoin, ClientID = 11, Data = 1 };
            queueMH.QueuedMessages.Enqueue(msg);
            stubNetworkSession.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(msg, incomingMessageQueue.ReadWholeMessage(MessageType.SuccessfulJoin));
        }

        [Test]
        public void CanQueueMoreThanOneMessage()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            IncomingMessageQueue mq = new IncomingMessageQueue(stubNS);
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[3] { 1, 2, 3 } });
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[2] { 4, 5 } });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadWholeMessage(MessageType.Player).Data);
            Assert.AreEqual(new byte[2] { 4, 5 }, mq.ReadWholeMessage(MessageType.Player).Data);
        }

        [Test]
        public void StoresSeperateQueuesForDifferentMessageTypes()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            var mq = new IncomingMessageQueue(stubNS);
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[3] { 1, 2, 3 } });
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Event, Data = new byte[2] { 4, 5 } });
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Player, Data = new byte[3] { 6, 7, 8 } });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadWholeMessage(MessageType.Player).Data);
            Assert.AreEqual(new byte[3] { 6, 7, 8 }, mq.ReadWholeMessage(MessageType.Player).Data);
            Assert.IsNull(mq.ReadWholeMessage(MessageType.Player));
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
        public delegate T DoDelegate(Arg arg);
        private T getNextQueuedMessage(Arg arg)
        {
            if (QueuedMessages.Count > 0)
            {
                return QueuedMessages.Dequeue();
            }
            return null;
        }
    }
}