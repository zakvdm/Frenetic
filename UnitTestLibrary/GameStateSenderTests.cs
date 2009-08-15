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
        IChatLogDiffer stubChatLogDiffer;
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
            client.Player.Stub(me => me.CurrentWeapon).Return(MockRepository.GenerateStub<IRailGun>());
            stubClientStateTracker.NetworkClients.Add(client);
            stubChatLogDiffer = MockRepository.GenerateStub<IChatLogDiffer>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubSnapCounter.CurrentSnap = 3;
            stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            serverChatLogView = new GameStateSender(stubChatLogDiffer, stubClientStateTracker, stubSnapCounter, stubOutgoingMessageQueue);
        }

        [Test]
        public void GenerateFlushesOutgoingQueueAtEnd()
        {
            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(me => me.SendMessagesOnQueue(), x => x.Repeat.Once());
        }

        [Test]
        public void GenerateSendsCurrentServerSnap()
        {
            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Items[0].Type == ItemType.ServerSnap && (int)y.Items[0].Data == 3), Arg<Client>.Is.Equal(client)));
        }

        [Test]
        public void GenerateSendsLastReceivedClientSnap()
        {
            client.LastClientSnap = 78;

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Items[0].Type == ItemType.ClientSnap && (int)y.Items[0].Data == 78), Arg<Client>.Is.Equal(client)));
        }

        [Test]
        public void GenerateSendsCurrentSnapAndClientNameWithChatMessage()
        {
            Log<ChatMessage> chatLog = new Log<ChatMessage>();
            ChatMessage chatMsg = new ChatMessage() { ClientName = "terence", Snap = 32 };
            chatLog.AddMessage(chatMsg);
            stubChatLogDiffer.Stub(x => x.GetOldestToYoungestDiff(client)).Return(chatLog);

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Items[0].Type == ItemType.ChatLog && ((ChatMessage)y.Items[0].Data).ClientName == "terence" && ((ChatMessage)y.Items[0].Data).Snap == 32), Arg<Client>.Is.Equal(client)));
        }

        [Test]
        public void GenerateSendsChatMessageLogOneMessageAtATimeToClient()
        {
            Log<ChatMessage> chatLog = new Log<ChatMessage>();
            chatLog.AddMessage(new ChatMessage() { Message = "Woohoo" });
            chatLog.AddMessage(new ChatMessage() { Message = "boohoo" });
            stubChatLogDiffer.Stub(x => x.GetOldestToYoungestDiff(client)).Return(chatLog);

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Items[0].Type == ItemType.ChatLog && ((ChatMessage)y.Items[0].Data).Message == "Woohoo"), Arg<Client>.Is.Equal(client)));
            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Items[0].Type == ItemType.ChatLog && ((ChatMessage)y.Items[0].Data).Message == "boohoo"), Arg<Client>.Is.Equal(client)));
        }

       

        [Test]
        public void GetsADiffedLogForEachClient()
        {
            client.ID = 1;
            Client client2 = new Client(client.Player) { ID = 2 };
            stubClientStateTracker.NetworkClients.Add(client2);

            serverChatLogView.Generate();

            stubChatLogDiffer.AssertWasCalled(x => x.GetOldestToYoungestDiff(client));
            stubChatLogDiffer.AssertWasCalled(x => x.GetOldestToYoungestDiff(client2));
        }

        [Test]
        public void SendsPlayerStateToAllClients()
        {
            client.ID = 7;
            client.Player = MockRepository.GenerateStub<IPlayer>();
            client.Player.Position = new Vector2(100, 200);
            client.Player.Stub(me => me.CurrentWeapon).Return(new RailGun(null));

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.AddToQueue(Arg<Item>.Matches(y => y.Type == ItemType.Player && y.ClientID == 7 && ((IPlayerState)y.Data).Position == new Vector2(100, 200))));
        }

        [Test]
        public void SendsPlayerSettingsToAllClients()
        {
            client.ID = 123;
            client.Player = MockRepository.GenerateStub<IPlayer>();
            var playerSettings = MockRepository.GenerateStub<IPlayerSettings>();
            client.Player.Stub(me => me.PlayerSettings).Return(playerSettings);
            client.Player.Stub(me => me.CurrentWeapon).Return(new RailGun(null));

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.AddToQueue(Arg<Item>.Matches(y => y.Type == ItemType.PlayerSettings && y.ClientID == 123 && ((IPlayerSettings)y.Data == playerSettings))));
        }
    }
}
