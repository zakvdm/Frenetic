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
        QueuedMessageHelper<Message, MessageType> clientSnapQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> chatLogQueueMessageHelper;
        IClientStateTracker stubClientStateTracker;
        IChatLogDiffer stubChatLogDiffer;
        ISnapCounter stubSnapCounter;
        Log<ChatMessage> serverLog;
        ClientInputProcessor clientInputProcessor;

        [SetUp]
        public void SetUp()
        {
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            serverSnapQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            clientSnapQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            chatLogQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ServerSnap))).Do(serverSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ClientSnap))).Do(clientSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            stubChatLogDiffer = MockRepository.GenerateStub<IChatLogDiffer>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            serverLog = new Log<ChatMessage>();
            clientInputProcessor = new ClientInputProcessor(serverLog, stubClientStateTracker, stubChatLogDiffer, stubSnapCounter, stubIncomingMessageQueue);
        }

        [Test]
        public void ChecksThatClientInputChatMessageIsNewBeforeAddingToServerLog()
        {
            ChatMessage chatMsg = new ChatMessage();
            stubClientStateTracker.Stub(x => x[Arg<int>.Is.Anything]).Return(new Client());
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = chatMsg });

            clientInputProcessor.Process(1);

            stubChatLogDiffer.AssertWasCalled(x => x.IsNewClientChatMessage(chatMsg));
        }

        [Test]
        public void UpdatesChatLogFromClientInput()
        {
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { Data = new ChatMessage() { Message = "client message" } });
            stubClientStateTracker.Stub(x => x[Arg<int>.Is.Anything]).Return(new Client());
            stubChatLogDiffer.Stub(x => x.IsNewClientChatMessage(Arg<ChatMessage>.Is.Anything)).Return(true);

            clientInputProcessor.Process(1);

            Assert.AreEqual("client message", serverLog[0].Message);
        }

        [Test]
        public void UpdatesAcknowledgedServerSnapNumbersPerClient()
        {
            Client client = new Client();
            stubClientStateTracker.Stub(x => x[3]).Return(client);
            serverSnapQueueMessageHelper.QueuedMessages.Enqueue(new Message() { ClientID = 3, Type = MessageType.ServerSnap, Data = 12 });

            clientInputProcessor.Process(1);

            Assert.AreEqual(12, client.LastServerSnap);
        }

        [Test]
        public void UpdatesCurrentClientSnapPerClient()
        {
            Client client = new Client();
            stubClientStateTracker.Stub(x => x[3]).Return(client);
            stubChatLogDiffer.Stub(x => x.IsNewClientChatMessage(Arg<ChatMessage>.Is.Anything)).Return(true);
            clientSnapQueueMessageHelper.QueuedMessages.Enqueue(new Message() { ClientID = 3, Type = MessageType.ClientSnap, Data = 32 });

            clientInputProcessor.Process(1);

            Assert.AreEqual(32, client.LastClientSnap);
        }

        [Test]
        public void AddsClientNameToNewChatMessages()
        {
            Client client = new Client() { Name = "terence" };
            stubClientStateTracker.Stub(x => x[1]).Return(client);
            stubChatLogDiffer.Stub(x => x.IsNewClientChatMessage(Arg<ChatMessage>.Is.Anything)).Return(true);
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { ClientID = 1, Type = MessageType.ChatLog, Data = new ChatMessage() { Message = "I am AWESOME" } });

            clientInputProcessor.Process(1);

            Assert.AreEqual("terence", serverLog[0].ClientName);
        }

        [Test]
        public void AddsNewChatMessagesWithCurrentServerSnap()
        {
            Client client = new Client();
            stubClientStateTracker.Stub(x => x[1]).Return(client);
            stubChatLogDiffer.Stub(x => x.IsNewClientChatMessage(Arg<ChatMessage>.Is.Anything)).Return(true);
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Message() { ClientID = 1, Type = MessageType.ChatLog, Data = new ChatMessage() { Message = "I am AWESOME" } });
            stubSnapCounter.CurrentSnap = 331;

            clientInputProcessor.Process(1);

            Assert.AreEqual(331, serverLog[0].Snap);
        }
    }
}
