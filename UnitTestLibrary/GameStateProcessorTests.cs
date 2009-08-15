using System;
using NUnit.Framework;
using Frenetic.Network;
using Rhino.Mocks;
using Frenetic;
using Frenetic.Player;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameStateProcessorTests
    {
        LocalClient client;
        Log<ChatMessage> clientLog;
        QueuedMessageHelper<Item, ItemType> chatLogQueueMessageHelper;
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
            playerQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            playerSettingsMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.ChatLog)).Do(chatLogQueueMessageHelper.HasMessageAvailable);
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
        public void UpdatesChatLogBasedOnMessage()
        {
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Data = new List<ChatMessage>() { new ChatMessage() { Message = "I'm a newer msg that came from the server" }, new ChatMessage() { Message = "I'm a msg that came from the server" } } });

            chatLogProcessor.Process(1);

            Assert.AreEqual("I'm a msg that came from the server", clientLog[1].Message);
            Assert.AreEqual("I'm a newer msg that came from the server", clientLog[0].Message);
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
