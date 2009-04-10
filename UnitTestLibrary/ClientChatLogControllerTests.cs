using System;
using NUnit.Framework;
using Frenetic.Network;
using Rhino.Mocks;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientChatLogControllerTests
    {
        Client client;
        MessageLog clientLog;
        QueuedMessageHelper<Message, MessageType> chatLogQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> serverSnapQueueMessageHelper;
        IIncomingMessageQueue stubIncomingMessageQueue;
        ClientChatLogController clientChatLogController;

        [SetUp]
        public void SetUp()
        {
            client = new Client();
            clientLog = new MessageLog();
            chatLogQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            serverSnapQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ServerSnap))).Do(serverSnapQueueMessageHelper.GetNextQueuedMessage);
            clientChatLogController = new ClientChatLogController(client, clientLog, stubIncomingMessageQueue);
        }

        [Test]
        public void HandlesEmptyMessageQueueCorrectly()
        {
            clientChatLogController.Process(1);

            stubIncomingMessageQueue.AssertWasCalled(x => x.ReadWholeMessage(MessageType.ChatLog));
        }

        [Test]
        public void UpdatesChatLogBasedOnMessage()
        {
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = "I'm a msg that came from the server" });
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = "I'm a newer msg that came from the server" });

            clientChatLogController.Process(1);

            Assert.AreEqual("I'm a msg that came from the server", clientLog[1]);
            Assert.AreEqual("I'm a newer msg that came from the server", clientLog[0]);
        }

        [Test]
        public void UpdatesTheServerSnapForThisClient()
        {
            Assert.AreNotEqual(101, client.LastServerSnap);
            serverSnapQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Type = MessageType.ServerSnap, Data = 101 });

            clientChatLogController.Process(1);

            Assert.AreEqual(101, client.LastServerSnap);
        }

    }
}
