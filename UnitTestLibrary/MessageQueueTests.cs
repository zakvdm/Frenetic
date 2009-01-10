﻿using System;
using System.Collections.Generic;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using System.Xml.Serialization;
using System.IO;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MessageQueueTests
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
            MessageQueue mq = new MessageQueue(stubNS);

            Assert.IsNotNull(mq);
        }

        [Test]
        public void EmptyQueueReturnsFalseOnMessageAvailable()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            MessageQueue mq = new MessageQueue(stubNS);
            stubNS.Stub(x => x.ReadMessage()).Return(null);

            Assert.IsNull(mq.ReadMessage(MessageType.PlayerData));
        }

        [Test]
        public void ReturnsMessageWhenAvailable()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            MessageQueue mq = new MessageQueue(stubNS);
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.PlayerData, Data = new byte[3] { 1, 2, 3 } });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadMessage(MessageType.PlayerData));
        }

        [Test]
        public void CanQueueMoreThanOneMessage()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            MessageQueue mq = new MessageQueue(stubNS);
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.PlayerData, Data = new byte[3] { 1, 2, 3 } });
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.PlayerData, Data = new byte[2] { 4, 5 } });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadMessage(MessageType.PlayerData));
            Assert.AreEqual(new byte[2] { 4, 5 }, mq.ReadMessage(MessageType.PlayerData));
        }

        [Test]
        public void StoresSeperateQueuesForDifferentIDs()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            var mq = new MessageQueue(stubNS);
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.PlayerData, Data = new byte[3] { 1, 2, 3 } });
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.Event, Data = new byte[2] { 4, 5 } });
            queueMH.QueuedMessages.Enqueue(new Message() { Type = MessageType.PlayerData, Data = new byte[3] { 6, 7, 8 } });
            stubNS.Stub(x => x.ReadMessage()).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(new byte[3] { 1, 2, 3 }, mq.ReadMessage(MessageType.PlayerData));
            Assert.AreEqual(new byte[3] { 6, 7, 8 }, mq.ReadMessage(MessageType.PlayerData));
            Assert.IsNull(mq.ReadMessage(MessageType.PlayerData));
        }
    }

    public class QueuedMessageHelper<T> where T:class
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
            return null;
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