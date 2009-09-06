using System;
using NUnit.Framework;
using Frenetic;
using Frenetic.Network;
using Rhino.Mocks;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Frenetic.Player;
using System.Collections.Generic;
using Frenetic.Engine;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientInputSenderTests
    {
        IOutgoingMessageQueue stubOutgoingMessageQueue;
        LocalClient client;
        ISnapCounter stubSnapCounter;
        Log<ChatMessage> chatLog;
        ClientInputSender clientInputSender;

        [SetUp]
        public void SetUp()
        {
            stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubSnapCounter.CurrentSnap = 3;
            chatLog = new Log<ChatMessage>();
            client = new LocalClient(MockRepository.GenerateStub<IPlayer>()) { ID = 9 };
            client.Player.Stub(me => me.PlayerSettings).Return(MockRepository.GenerateStub<IPlayerSettings>());
            clientInputSender = new ClientInputSender(client, chatLog, stubSnapCounter, stubOutgoingMessageQueue, DummyLogger.Factory); 
        }

        [Test]
        public void DoesNothingWhenClientIDIsZero()
        {
            client.ID = 0;

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasNotCalled(me => me.AddToQueue(Arg<Item>.Is.Anything));
        }

        [Test]
        public void ChecksForANewSnapBeforeSending()
        {
            stubSnapCounter.CurrentSnap = 0;

            clientInputSender.Generate();
            stubOutgoingMessageQueue.AssertWasNotCalled(me => me.AddToQueue(Arg<Item>.Is.Anything));
            stubSnapCounter.CurrentSnap = 2;
            
            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(me => me.AddToQueue(Arg<Item>.Is.Anything), o => o.Repeat.AtLeastOnce());
        }

        [Test]
        public void FlushesTheOutgoingQueue()
        {
            stubSnapCounter.CurrentSnap = 100;

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(me => me.SendMessagesOnQueue());
        }

        [Test]
        public void SendsChatLogMessages()
        {
            stubSnapCounter.CurrentSnap = 24;
            chatLog.AddMessage(new ChatMessage() { Message = "new message 1" });
            chatLog.AddMessage(new ChatMessage() { Message = "new message 2" });
            chatLog.AddMessage(new ChatMessage() { Message = "new message 3" });

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(me => me.AddToReliableQueue(Arg<Item>.Matches(y => y.ClientID == 9 && y.Type == ItemType.ChatLog && ((List<ChatMessage>)y.Data).Count == 3 && ((List<ChatMessage>)y.Data)[0].Message == "new message 3")), o => o.Repeat.Once());
        }

        [Test]
        public void SendsLocalPlayer()
        {
            client.Player.Position = new Vector2(100, 200);

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.AddToQueue(Arg<Item>.Matches(y => y.Type == ItemType.PlayerInput && ((IPlayer)y.Data).Position == new Vector2(100, 200))));
        }
        [Test]
        public void OnlySendsPendingShotOnce()
        {
            client.Player.PendingShot = new Vector2(10, 20);

            clientInputSender.Generate();

            Assert.IsNull(client.Player.PendingShot);
        }

        [Test]
        public void SendsLocalPlayerSettings()
        {
            client.Player.PlayerSettings.Name = "zak";
            client.Player.PlayerSettings.Stub(me => me.IsDirty).Return(true);
            client.Player.PlayerSettings.Stub(me => me.GetDiff()).Return(client.Player.PlayerSettings);

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.AddToReliableQueue(Arg<Item>.Matches(y => y.Type == ItemType.PlayerSettings && ((IPlayerSettings)y.Data).Name == "zak")));
        }
    }
}
