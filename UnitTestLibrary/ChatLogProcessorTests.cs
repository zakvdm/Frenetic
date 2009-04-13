using System;
using NUnit.Framework;
using Frenetic.Network;
using Rhino.Mocks;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ChatLogProcessorTests
    {
        Client client;
        Log<ChatMessage> clientLog;
        QueuedMessageHelper<Message, MessageType> chatLogQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> serverSnapQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> clientSnapQueueMessageHelper;
        IIncomingMessageQueue stubIncomingMessageQueue;
        ChatLogProcessor chatLogProcessor;

        [SetUp]
        public void SetUp()
        {
            client = new Client();
            clientLog = new Log<ChatMessage>();
            chatLogQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            serverSnapQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            clientSnapQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ServerSnap))).Do(serverSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ClientSnap))).Do(clientSnapQueueMessageHelper.GetNextQueuedMessage);
            chatLogProcessor = new ChatLogProcessor(client, clientLog, stubIncomingMessageQueue);
        }

        [Test]
        public void HandlesEmptyMessageQueueCorrectly()
        {
            chatLogProcessor.Process(1);

            stubIncomingMessageQueue.AssertWasCalled(x => x.ReadWholeMessage(MessageType.ChatLog));
        }

        [Test]
        public void UpdatesTheLastReceivedServerSnapForThisClient()
        {
            Assert.AreNotEqual(101, client.LastServerSnap);
            serverSnapQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Type = MessageType.ServerSnap, Data = 101 });

            chatLogProcessor.Process(1);

            Assert.AreEqual(101, client.LastServerSnap);
        }

        [Test]
        public void UpdatesTheLastAcknowledgedClientSnapFromTheServer()
        {
            Assert.AreNotEqual(99, client.LastClientSnap);
            clientSnapQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Type = MessageType.ClientSnap, Data = 99 });

            chatLogProcessor.Process(1);

            Assert.AreEqual(99, client.LastClientSnap);
        }

        [Test]
        public void UpdatesChatLogBasedOnMessage()
        {
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = new ChatMessage() { Message = "I'm a msg that came from the server" } });
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = new ChatMessage() { Message = "I'm a newer msg that came from the server" } });

            chatLogProcessor.Process(1);

            Assert.AreEqual("I'm a msg that came from the server", clientLog[1].Message);
            Assert.AreEqual("I'm a newer msg that came from the server", clientLog[0].Message);
        }
        [Test]
        public void DoesntAddChatMessagesTwice()
        {
            ChatMessage msg1 = new ChatMessage() { ClientName = "terence", Snap = 12, Message = "i like boys" };
            ChatMessage msg2 = new ChatMessage() { ClientName = "zak", Snap = 13, Message = "i'm a boy..." };
            clientLog.AddMessage(msg1);
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = msg1 });
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = msg2 });

            chatLogProcessor.Process(1);

            Assert.AreEqual(2, clientLog.Count);
            Assert.AreEqual(msg1, clientLog[1]);
            Assert.AreEqual(msg2, clientLog[0]);
        }

    }
}
