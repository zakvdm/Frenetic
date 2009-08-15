using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network;
using Frenetic;
using Microsoft.Xna.Framework;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientInputProcessorTests
    {
        IIncomingMessageQueue stubIncomingMessageQueue;
        QueuedMessageHelper<Item, ItemType> serverSnapQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> clientSnapQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> chatLogQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> playerQueueMessageHelper;
        QueuedMessageHelper<Item, ItemType> playerSettingsQueueMessageHelper;
        IClientStateTracker stubClientStateTracker;
        IChatLogDiffer stubChatLogDiffer;
        ISnapCounter stubSnapCounter;
        INetworkPlayerProcessor stubNetworkPlayerProcessor;
        Log<ChatMessage> serverLog;
        Client client;
        ClientInputProcessor clientInputProcessor;

        [SetUp]
        public void SetUp()
        {
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            serverSnapQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            clientSnapQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            chatLogQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            playerQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            playerSettingsQueueMessageHelper = new QueuedMessageHelper<Item, ItemType>();
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.ServerSnap))).Do(serverSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.ServerSnap)).Do(serverSnapQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.ClientSnap))).Do(clientSnapQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.ClientSnap)).Do(clientSnapQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.ChatLog))).Do(chatLogQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.ChatLog)).Do(chatLogQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.Player))).Do(playerQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.Player)).Do(playerQueueMessageHelper.HasMessageAvailable);
            stubIncomingMessageQueue.Stub(x => x.ReadItem(Arg<ItemType>.Is.Equal(ItemType.PlayerSettings))).Do(playerSettingsQueueMessageHelper.GetNextQueuedMessage);
            stubIncomingMessageQueue.Stub(x => x.HasAvailable(ItemType.PlayerSettings)).Do(playerSettingsQueueMessageHelper.HasMessageAvailable);
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            stubChatLogDiffer = MockRepository.GenerateStub<IChatLogDiffer>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubNetworkPlayerProcessor = MockRepository.GenerateStub<INetworkPlayerProcessor>();
            serverLog = new Log<ChatMessage>();
            client = new Client(MockRepository.GenerateStub<IPlayer>());
            client.Player.Stub(me => me.PlayerSettings).Return(MockRepository.GenerateStub<IPlayerSettings>());
            clientInputProcessor = new ClientInputProcessor(stubNetworkPlayerProcessor, serverLog, stubClientStateTracker, stubChatLogDiffer, stubSnapCounter, stubIncomingMessageQueue);
        }

        [Test]
        public void ChecksThatClientInputChatMessageIsNewBeforeAddingToServerLog()
        {
            ChatMessage chatMsg = new ChatMessage();
            stubClientStateTracker.Stub(x => x.FindNetworkClient(Arg<int>.Is.Anything)).Return(client);
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Data = chatMsg });

            clientInputProcessor.Process(1);

            stubChatLogDiffer.AssertWasCalled(x => x.IsNewClientChatMessage(chatMsg));
        }

        [Test]
        public void UpdatesChatLogFromClientInput()
        {
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { Data = new ChatMessage() { Message = "client message" } });
            stubClientStateTracker.Stub(x => x.FindNetworkClient(Arg<int>.Is.Anything)).Return(client);
            stubChatLogDiffer.Stub(x => x.IsNewClientChatMessage(Arg<ChatMessage>.Is.Anything)).Return(true);

            clientInputProcessor.Process(1);

            Assert.AreEqual("client message", serverLog[0].Message);
        }

        [Test]
        public void UpdatesAcknowledgedServerSnapNumbersPerClient()
        {
            stubClientStateTracker.Stub(x => x.FindNetworkClient(3)).Return(client);
            serverSnapQueueMessageHelper.QueuedMessages.Enqueue(new Item() { ClientID = 3, Type = ItemType.ServerSnap, Data = 12 });

            clientInputProcessor.Process(1);

            Assert.AreEqual(12, client.LastServerSnap);
        }

        [Test]
        public void UpdatesCurrentClientSnapPerClient()
        {
            stubClientStateTracker.Stub(x => x.FindNetworkClient(3)).Return(client);
            stubChatLogDiffer.Stub(x => x.IsNewClientChatMessage(Arg<ChatMessage>.Is.Anything)).Return(true);
            clientSnapQueueMessageHelper.QueuedMessages.Enqueue(new Item() { ClientID = 3, Type = ItemType.ClientSnap, Data = 32 });

            clientInputProcessor.Process(1);

            Assert.AreEqual(32, client.LastClientSnap);
        }

        [Test]
        public void AddsClientNameToNewChatMessages()
        {
            client.Player.PlayerSettings.Name = "terence";
            stubClientStateTracker.Stub(x => x.FindNetworkClient(1)).Return(client);
            stubChatLogDiffer.Stub(x => x.IsNewClientChatMessage(Arg<ChatMessage>.Is.Anything)).Return(true);
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { ClientID = 1, Type = ItemType.ChatLog, Data = new ChatMessage() { Message = "I am AWESOME" } });

            clientInputProcessor.Process(1);

            Assert.AreEqual("terence", serverLog[0].ClientName);
        }

        [Test]
        public void AddsNewChatMessagesWithCurrentServerSnap()
        {
            stubClientStateTracker.Stub(x => x.FindNetworkClient(1)).Return(client);
            stubChatLogDiffer.Stub(x => x.IsNewClientChatMessage(Arg<ChatMessage>.Is.Anything)).Return(true);
            chatLogQueueMessageHelper.QueuedMessages.Enqueue(new Item() { ClientID = 1, Type = ItemType.ChatLog, Data = new ChatMessage() { Message = "I am AWESOME" } });
            stubSnapCounter.CurrentSnap = 331;

            clientInputProcessor.Process(1);

            Assert.AreEqual(331, serverLog[0].Snap);
        }

        [Test]
        public void UpdatesPlayer()
        {
            stubClientStateTracker.Stub(x => x.FindNetworkClient(3)).Return(client);
            var receivedPlayer = MockRepository.GenerateStub<IPlayer>();
            receivedPlayer.Position = new Vector2(-31, -92);
            var item = new Item() { ClientID = 3, Type = ItemType.Player, Data = receivedPlayer };
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
