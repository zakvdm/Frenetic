using System;
using NUnit.Framework;
using Frenetic.Network;
using Rhino.Mocks;
using Frenetic;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameStateProcessorTests
    {
        LocalClient client;
        Log<ChatMessage> clientLog;
        QueuedMessageHelper<Item, ItemType> chatLogQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> serverSnapQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> clientSnapQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> playerQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> playerSettingsMessageHelper;
        INetworkPlayerProcessor stubNetworkPlayerProcessor;
        IClientStateTracker stubClientStateTracker;
        IIncomingMessageQueue stubIncomingMessageQueue;
        GameStateProcessor chatLogProcessor;

        [SetUp]
        public void SetUp()
        {
            client = new LocalClient(null);
            clientLog = new Log<ChatMessage>();
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            chatLogQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            serverSnapQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            clientSnapQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            playerQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            playerSettingsMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.ChatLog)).Do(chatLogQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.ServerSnap))).Do(serverSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.ServerSnap)).Do(serverSnapQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.ClientSnap))).Do(clientSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.ClientSnap)).Do(clientSnapQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.Player))).Do(playerQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.Player)).Do(playerQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.PlayerSettings))).Do(playerSettingsMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.PlayerSettings)).Do(playerSettingsMessageHelper.HasMessageAvailable);
            stubNetworkPlayerProcessor = MockRepository.GenerateStub<INetworkPlayerProcessor>();
            chatLogProcessor = new GameStateProcessor(client, clientLog, stubNetworkPlayerProcessor, stubClientStateTracker, stubIncomingMessageQueue);
        }

        [Test]
        public void HandlesEmptyMessageQueueCorrectly()
        {
            chatLogProcessor.Process(1);

            stubIncomingMessageQueue.AssertWasNotCalled(x => x.ReadItem(ItemType.ChatLog));
        }

        [Test]
        public void UpdatesTheLastReceivedServerSnapForThisClient()
        {
            Assert.AreNotEqual(101, client.LastServerSnap);
            serverSnapQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Type = ItemType.ServerSnap, Data = 101 });

            chatLogProcessor.Process(1);

            Assert.AreEqual(101, client.LastServerSnap);
        }

        [Test]
        public void UpdatesTheLastAcknowledgedClientSnapFromTheServer()
        {
            Assert.AreNotEqual(99, client.LastClientSnap);
            clientSnapQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Type = ItemType.ClientSnap, Data = 99 });

            chatLogProcessor.Process(1);

            Assert.AreEqual(99, client.LastClientSnap);
        }

        [Test]
        public void UpdatesChatLogBasedOnMessage()
        {
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Data = new ChatMessage() { Message = "I'm a msg that came from the server" } });
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Data = new ChatMessage() { Message = "I'm a newer msg that came from the server" } });

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
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Data = msg1 });
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Data = msg2 });

            chatLogProcessor.Process(1);

            Assert.AreEqual(2, clientLog.Count);
            Assert.AreEqual(msg1, clientLog[1]);
            Assert.AreEqual(msg2, clientLog[0]);
        }

        [Test]
        public void UpdatesPlayer()
        {
            stubClientStateTracker.Stub(x => x.FindNetworkClient(3)).Return(client);
            var receivedState = MockRepository.GenerateStub<IPlayerState>();
            var item = new Item() { ClientID = 3, Type = ItemType.Player, Data = receivedState };
            playerQueueMessageHelper.QueuedMessages.Enqueue(item);

            chatLogProcessor.Process(1);

            stubNetworkPlayerProcessor.AssertWasCalled(me => me.UpdatePlayerFromPlayerStateItem(Arg<Item>.Is.Equal(item)));
        }

        [Test]
        public void UpdatesPlayerSettings()
        {
            stubClientStateTracker.Stub(x => x.FindNetworkClient(3)).Return(client);
            NetworkPlayerSettings receivedPlayerSettings = new NetworkPlayerSettings();
            var item = new Item() { ClientID = 3, Type = ItemType.Player, Data = receivedPlayerSettings };
            playerSettingsMessageHelper.QueuedMessages.Enqueue(item);

            chatLogProcessor.Process(1);

            stubNetworkPlayerProcessor.AssertWasCalled(me => me.UpdatePlayerSettingsFromNetworkItem(Arg<Item>.Is.Equal(item)));
        }
    }
}
