using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientInputProcessorTests
    {
        IIncomingMessageQueue stubIncomingMessageQueue;
        QueuedMessageHelper<Message, MessageType> serverSnapQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> chatLogQueueMessageHelper;
        IClientStateTracker stubClientStateTracker;
        MessageLog serverLog;
        ClientInputProcessor networkClientInputController;

        [SetUp]
        public void SetUp()
        {
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            serverSnapQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            chatLogQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ServerSnap))).Do(serverSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            serverLog = new MessageLog();
            networkClientInputController = new ClientInputProcessor(serverLog, stubClientStateTracker, stubIncomingMessageQueue);
        }

        [Test]
        public void UpdatesChatLogFromClientInput()
        {
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = "client message" });
            stubClientStateTracker.Stub(x => x[Arg<int>.Is.Anything]).Return(new Client());

            networkClientInputController.Process(1);

            Assert.AreEqual("client message", serverLog[0]);
        }

        [Test]
        public void UpdatesAcknowledgedServerSnapNumbersPerClient()
        {
            Client client = new Client();
            stubClientStateTracker.Stub(x => x[3]).Return(client);
            serverSnapQueueMessageHelper.QueuedMessages.Enqueue(new Message() { ClientID = 3, Type = MessageType.ServerSnap, Data = 12 });

            networkClientInputController.Process(1);

            Assert.AreEqual(12, client.LastServerSnap);
        }
    }
}
