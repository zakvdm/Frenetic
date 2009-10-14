using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic;
using Frenetic.Network;
using Lidgren.Network;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Frenetic.Player;
using Frenetic.Weapons;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameStateSenderTests
    {
        IClientStateTracker stubClientStateTracker;
        Client client;
        Log<ChatMessage> chatLog;
        ISnapCounter stubSnapCounter;
        IOutgoingMessageQueue stubOutgoingMessageQueue;
        GameStateSender serverChatLogView;

        [SetUp]
        public void SetUp()
        {
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            List<Client> clientList = new List<Client>();
            stubClientStateTracker.Stub(x => x.NetworkClients).Return(clientList);
            client = new Client(MockRepository.GenerateStub<IPlayer>());
            client.Player.Stub(me => me.PlayerSettings).Return(MockRepository.GenerateStub<IPlayerSettings>());
            client.Player.Stub(me => me.CurrentWeapon).Return(MockRepository.GenerateStub<IWeapon>());
            client.Player.CurrentWeapon.Stub(me => me.Shots).Return(new Shots());
            stubClientStateTracker.NetworkClients.Add(client);
            chatLog = new Log<ChatMessage>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubSnapCounter.CurrentSnap = 3;
            stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            serverChatLogView = new GameStateSender(chatLog, stubClientStateTracker, stubSnapCounter, stubOutgoingMessageQueue);
        }

        [Test]
        public void GenerateFlushesOutgoingQueueAtEnd()
        {
            serverChatLogView.Generate(1f);

            stubOutgoingMessageQueue.AssertWasCalled(me => me.SendMessagesOnQueue(), x => x.Repeat.Once());
        }

        [Test]
        public void GenerateSendsClientNameWithChatMessage()
        {
            ChatMessage chatMsg = new ChatMessage() { ClientName = "terence", Message = "Test" };
            chatLog.Add(chatMsg);

            serverChatLogView.Generate(1f);

            stubOutgoingMessageQueue.AssertWasCalled(x => x.AddToReliableQueue(Arg<Item>.Matches(y => y.Type == ItemType.ChatLog && ((List<ChatMessage>)y.Data)[0].ClientName == "terence")));
        }

        [Test]
        public void GenerateSendsNewChatMessages()
        {
            chatLog.Add(new ChatMessage() { Message = "Woohoo" });
            chatLog.Add(new ChatMessage() { Message = "boohoo" });

            serverChatLogView.Generate(1f);

            stubOutgoingMessageQueue.AssertWasCalled(me => me.AddToReliableQueue(Arg<Item>.Matches(y => (y.Type == ItemType.ChatLog) && ((List<ChatMessage>)y.Data).Count == 2 && ((List<ChatMessage>)y.Data)[0].Message == "boohoo")));
        }

        [Test]
        public void SendsPlayerStateToAllClients()
        {
            client.ID = 7;
            client.Player = MockRepository.GenerateStub<IPlayer>();
            client.Player.Stub(me => me.PlayerSettings).Return(MockRepository.GenerateStub<IPlayerSettings>());
            client.Player.Position = new Vector2(100, 200);
            client.Player.Stub(me => me.CurrentWeapon).Return(new RailGun(null));

            serverChatLogView.Generate(1f);

            stubOutgoingMessageQueue.AssertWasCalled(x => x.AddToQueue(Arg<Item>.Matches(y => y.Type == ItemType.Player && y.ClientID == 7 && ((IPlayerState)y.Data).Position == new Vector2(100, 200))));
        }

        [Test]
        public void SendsDirtyPlayerSettingsToAllClients()
        {
            client.ID = 123;
            client.Player = MockRepository.GenerateStub<IPlayer>();
            var playerSettings = MockRepository.GenerateStub<IPlayerSettings>();
            playerSettings.Stub(me => me.IsDirty).Return(true);
            playerSettings.Stub(me => me.GetDiff()).Return(playerSettings);
            client.Player.Stub(me => me.PlayerSettings).Return(playerSettings);
            client.Player.Stub(me => me.CurrentWeapon).Return(new RailGun(null));

            serverChatLogView.Generate(1f);

            stubOutgoingMessageQueue.AssertWasCalled(x => x.AddToReliableQueue(Arg<Item>.Matches(y => y.Type == ItemType.PlayerSettings && y.ClientID == 123 && ((IPlayerSettings)y.Data == playerSettings))));
        }
    }
}
