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
        LocalClient client;
        Log<ChatMessage> clientLog;
        QueuedMessageHelper<Message, MessageType> chatLogQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> serverSnapQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> clientSnapQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> playerQueueMessageHelper;
        QueuedMessageHelper<Message, MessageType> playerSettingsMessageHelper;
        INetworkPlayerProcessor stubNetworkPlayerProcessor;
        IClientStateTracker stubClientStateTracker;
        IIncomingMessageQueue stubIncomingMessageQueue;
        ChatLogProcessor chatLogProcessor;

        [SetUp]
        public void SetUp()
        {
            client = new LocalClient(null, null);
            clientLog = new Log<ChatMessage>();
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            chatLogQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            serverSnapQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            clientSnapQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            playerQueueMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            playerSettingsMessageHelper = new QueuedMessageHelper<Message, MessageType>();
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ServerSnap))).Do(serverSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.ClientSnap))).Do(clientSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.Player))).Do(playerQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.PlayerSettings))).Do(playerSettingsMessageHelper.GetNextQueuedMessage);
            stubNetworkPlayerProcessor = MockRepository.GenerateStub<INetworkPlayerProcessor>();
            chatLogProcessor = new ChatLogProcessor(client, clientLog, stubNetworkPlayerProcessor, stubClientStateTracker, stubIncomingMessageQueue);
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

        [Test]
        public void UpdatesPlayer()
        {
            stubClientStateTracker.Stub(x => x[3]).Return(client);
            Player receivedPlayer = new Player(null, null);
            Message msg = new Message() { ClientID = 3, Type = MessageType.Player, Data = receivedPlayer };
            playerQueueMessageHelper.QueuedMessages.Enqueue(msg);

            chatLogProcessor.Process(1);

            stubNetworkPlayerProcessor.AssertWasCalled(me => me.UpdatePlayerFromNetworkMessage(Arg<Message>.Is.Equal(msg)));
        }

        [Test]
        public void UpdatesPlayerSettings()
        {
            stubClientStateTracker.Stub(x => x[3]).Return(client);
            PlayerSettings receivedPlayerSettings = new PlayerSettings();
            Message msg = new Message() { ClientID = 3, Type = MessageType.Player, Data = receivedPlayerSettings };
            playerSettingsMessageHelper.QueuedMessages.Enqueue(msg);

            chatLogProcessor.Process(1);

            stubNetworkPlayerProcessor.AssertWasCalled(me => me.UpdatePlayerSettingsFromNetworkMessage(Arg<Message>.Is.Equal(msg)));
        }
    }
}
