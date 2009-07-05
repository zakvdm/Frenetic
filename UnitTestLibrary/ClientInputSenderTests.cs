using System;
using NUnit.Framework;
using Frenetic;
using Frenetic.Network;
using Rhino.Mocks;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientInputSenderTests
    {
        IOutgoingMessageQueue stubOutgoingMessageQueue;
        LocalClient client;
        ISnapCounter stubSnapCounter;
        MessageConsole console;
        ClientInputSender clientInputSender;

        [SetUp]
        public void SetUp()
        {
            stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubSnapCounter.CurrentSnap = 3;
            console = new MessageConsole(null, new Log<ChatMessage>());
            client = new LocalClient(MockRepository.GenerateStub<IPlayer>(), new LocalPlayerSettings()) { ID = 9 };
            clientInputSender = new ClientInputSender(client, console, stubSnapCounter, stubOutgoingMessageQueue); 
        }

        [Test]
        public void DoesNothingWhenClientIDIsZero()
        {
            client.ID = 0;

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasNotCalled(me => me.Write(Arg<Message>.Is.Anything));
        }

        [Test]
        public void ChecksForANewSnapBeforeSending()
        {
            stubSnapCounter.CurrentSnap = 0;

            clientInputSender.Generate();
            stubOutgoingMessageQueue.AssertWasNotCalled(me => me.Write(Arg<Message>.Is.Anything));
            stubSnapCounter.CurrentSnap = 2;
            
            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(me => me.Write(Arg<Message>.Is.Anything), o => o.Repeat.AtLeastOnce());
        }

        [Test]
        public void SendsCurrentClientSnap()
        {
            stubSnapCounter.CurrentSnap = 99;

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ClientSnap && (int)y.Data == 99)));
        }

        [Test]
        public void SendsLastReceivedServerSnap()
        {
            client.LastServerSnap = 33;

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ServerSnap && (int)y.Data == 33)));
        }

        [Test]
        public void SendsChatLogMessages()
        {
            // Arrange:
            client.LastClientSnap = 23; // Set last acknowledged client snap
            stubSnapCounter.CurrentSnap = 20;
            console.ProcessInput("new message 1"); // Won't be sent
            clientInputSender.Generate();

            // Action:
            stubSnapCounter.CurrentSnap = 40;
            console.ProcessInput("new message 2");  // Will be sent (40 > 23)
            clientInputSender.Generate();

            stubSnapCounter.CurrentSnap = 80;
            console.ProcessInput("new message 3"); // Will send msg 3 and msg 2
            clientInputSender.Generate();

            // Assert:
            stubOutgoingMessageQueue.AssertWasCalled(me => me.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).Message == "new message 2")), o => o.Repeat.Twice());
            stubOutgoingMessageQueue.AssertWasCalled(me => me.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).Message == "new message 3")), o => o.Repeat.Once());
        }

        [Test]
        public void SendsTheClientSnapOnTheChatMessage()
        {
            // Arrange:
            client.LastClientSnap = 4;
            stubSnapCounter.CurrentSnap = 7;
            console.ProcessInput("hello");
            clientInputSender.Generate();
            stubSnapCounter.CurrentSnap = 8;

            // Action:
            clientInputSender.Generate(); // Even though we're at snap 8, the snap on the chat message should still be '7'

            // Assert:
            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).Snap == 7)), o => o.Repeat.Twice());
        }

        [Test]
        public void SendsLocalPlayer()
        {
            client.Player.Position = new Vector2(100, 200);

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.Type == MessageType.Player && ((IPlayer)y.Data).Position == new Vector2(100, 200))));
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
            client.PlayerSettings.Name = "zak";

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.Type == MessageType.PlayerSettings && ((LocalPlayerSettings)y.Data).Name == "zak")));
        }
    }
}
