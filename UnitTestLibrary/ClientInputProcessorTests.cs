using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network;
using Frenetic;
using Microsoft.Xna.Framework;
using Frenetic.Player;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientInputProcessorTests
    {
        IIncomingMessageQueue stubIncomingMessageQueue;
        QueuedMessageHelper<Item, ItemType> chatLogQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> playerQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> playerSettingsQueueMessageHelper;
        IClientStateTracker stubClientStateTracker;
        INetworkPlayerProcessor stubNetworkPlayerProcessor;
        Log<ChatMessage> serverLog;
        Client client;
        ClientInputProcessor clientInputProcessor;

        [SetUp]
        public void SetUp()
        {
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            chatLogQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            playerQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            playerSettingsQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.ChatLog)).Do(chatLogQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.PlayerInput))).Do(playerQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.PlayerInput)).Do(playerQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.PlayerSettings))).Do(playerSettingsQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.PlayerSettings)).Do(playerSettingsQueueMessageHelper.HasMessageAvailable);
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            stubNetworkPlayerProcessor = MockRepository.GenerateStub<INetworkPlayerProcessor>();
            serverLog = new Log<ChatMessage>();
            client = new Client(MockRepository.GenerateStub<IPlayer>());
            client.Player.Stub(me => me.PlayerSettings).Return(MockRepository.GenerateStub<IPlayerSettings>());
            clientInputProcessor = new ClientInputProcessor(stubNetworkPlayerProcessor, serverLog, stubClientStateTracker, stubIncomingMessageQueue);
        }

        [Test]
        public void UpdatesChatLogFromClientInput()
        {
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Data = new List<ChatMessage>() { new ChatMessage() { Message = "client message" } } });
            stubClientStateTracker.Stub(x => x.FindNetworkClient(Arg<int>.Is.Anything)).Return(client);

            clientInputProcessor.Process(1);

            Assert.AreEqual("client message", serverLog[0].Message);
        }

        [Test]
        public void AddsClientNameToNewChatMessages()
        {
            client.Player.PlayerSettings.Name = "terence";
            stubClientStateTracker.Stub(x => x.FindNetworkClient(1)).Return(client);
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { ClientID = 1, Type = ItemType.ChatLog, Data = new List<ChatMessage>() { new ChatMessage() { Message = "I am AWESOME" } } });

            clientInputProcessor.Process(1);

            Assert.AreEqual("terence", serverLog[0].ClientName);
        }

        [Test]
        public void UpdatesPlayer()
        {
            stubClientStateTracker.Stub(x => x.FindNetworkClient(3)).Return(client);
            var receivedPlayer = MockRepository.GenerateStub<IPlayer>();
            receivedPlayer.Position = new Vector2(-31, -92);
            var item = new Item() { ClientID = 3, Type = ItemType.PlayerInput, Data = receivedPlayer };
            playerQueueMessageHelper.QueuedMessages.Enqueue(item);

            clientInputProcessor.Process(1);

            stubNetworkPlayerProcessor.AssertWasCalled(me => me.UpdatePlayerFromNetworkItem(Arg<Item>.Is.Equal(item)));
        }

        [Test]
        public void UpdatesPlayerSettings()
        {
            stubClientStateTracker.Stub(x => x.FindNetworkClient(3)).Return(client);
            NetworkPlayerSettings receivedPlayerSettings = new NetworkPlayerSettings();
            var item = new Item() { ClientID = 3, Type = ItemType.Player, Data = receivedPlayerSettings };
            playerSettingsQueueMessageHelper.QueuedMessages.Enqueue(item);

            clientInputProcessor.Process(1);

            stubNetworkPlayerProcessor.AssertWasCalled(me => me.UpdatePlayerSettingsFromNetworkItem(Arg<Item>.Is.Equal(item)));
        }
    }
}
