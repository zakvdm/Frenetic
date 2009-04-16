using System;
using NUnit.Framework;
using Frenetic;
using Frenetic.Network;
using Rhino.Mocks;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientInputSenderTests
    {
        IOutgoingMessageQueue mockOutgoingMessageQueue;
        LocalClient client;
        ISnapCounter stubSnapCounter;
        MessageConsole console;
        ClientInputSender clientInputSender;

        [SetUp]
        public void SetUp()
        {
            mockOutgoingMessageQueue = MockRepository.GenerateMock<IOutgoingMessageQueue>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubSnapCounter.CurrentSnap = 3;
            console = new MessageConsole(null, new Log<ChatMessage>());
            client = new LocalClient(new Player(null, null), new PlayerSettings()) { ID = 9 };
            clientInputSender = new ClientInputSender(client, console, stubSnapCounter, mockOutgoingMessageQueue); 
        }

        [Test]
        public void DoesNothingWhenClientIDIsZero()
        {
            client.ID = 0;
            mockOutgoingMessageQueue.Expect(x => x.Write(Arg<Message>.Is.Anything)).Repeat.Never();

            clientInputSender.Generate();

            mockOutgoingMessageQueue.VerifyAllExpectations();
        }

        [Test]
        public void OnlySendsOncePerSnap()
        {
            stubSnapCounter.CurrentSnap = 2;
            clientInputSender.Generate(); // Send at current snap...

            // Arrange:
            mockOutgoingMessageQueue.Expect(x => x.Write(Arg<Message>.Is.Anything)).Repeat.Never(); // Once for ClientSnap and once for ServerSnap, but NO MORE!
            
            // Action:
            clientInputSender.Generate();

            // Assert:
            mockOutgoingMessageQueue.VerifyAllExpectations();
        }

        [Test]
        public void SendsCurrentClientSnap()
        {
            stubSnapCounter.CurrentSnap = 99;

            clientInputSender.Generate();

            mockOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ClientSnap && (int)y.Data == 99)));
        }

        [Test]
        public void SendsLastReceivedServerSnap()
        {
            client.LastServerSnap = 33;

            clientInputSender.Generate();

            mockOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ServerSnap && (int)y.Data == 33)));
        }

        [Test]
        public void SendsChatLogMessages()
        {
            // Arrange:
            mockOutgoingMessageQueue.Expect(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).Message == "new message 2"))).Repeat.Twice();
            mockOutgoingMessageQueue.Expect(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).Message == "new message 3"))).Repeat.Once();

            // Action:
            client.LastClientSnap = 23; // Set last acknowledged client snap
            stubSnapCounter.CurrentSnap = 20;
            console.ProcessInput("new message 1"); // Won't be sent
            clientInputSender.Generate();

            stubSnapCounter.CurrentSnap = 40;
            console.ProcessInput("new message 2");  // Will be sent (40 > 23)
            clientInputSender.Generate();

            stubSnapCounter.CurrentSnap = 80;
            console.ProcessInput("new message 3"); // Will send msg 3 and msg 2
            clientInputSender.Generate();

            // Assert:
            mockOutgoingMessageQueue.VerifyAllExpectations();
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
            mockOutgoingMessageQueue.Expect(x => x.Write(Arg<Message>.Matches(y => y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).Snap == 7))).Repeat.Once();

            // Action:
            clientInputSender.Generate();

            // Assert:
            mockOutgoingMessageQueue.VerifyAllExpectations();
        }

        [Test]
        public void SendsLocalPlayer()
        {
            Player player = new Player(null, null);
            player.Position = new Vector2(100, 200);
            client.Player = player;
            mockOutgoingMessageQueue.Expect(x => x.Write(Arg<Message>.Matches(y => y.Type == MessageType.Player && ((Player)y.Data).Position == new Vector2(100, 200)))).Repeat.Once();

            clientInputSender.Generate();

            mockOutgoingMessageQueue.VerifyAllExpectations();
        }

        [Test]
        public void SendsLocalPlayerSettings()
        {
            PlayerSettings playerSettings = new PlayerSettings();
            playerSettings.Name = "zak";
            client.PlayerSettings = playerSettings;
            mockOutgoingMessageQueue.Expect(x => x.Write(Arg<Message>.Matches(y => y.Type == MessageType.PlayerSettings && ((PlayerSettings)y.Data).Name == "zak"))).Repeat.Once();

            clientInputSender.Generate();

            mockOutgoingMessageQueue.VerifyAllExpectations();
        }
    }
}
