using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetworkPlayerControllerTests
    {
        IIncomingMessageQueue stubIncomingMessageQueue;
        NetworkPlayerController npController;
        QueuedMessageHelper<object, MessageType> queueMessageHelper;
        [SetUp]
        public void SetUp()
        {
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            npController = new NetworkPlayerController(stubIncomingMessageQueue);
            npController.Players.Add(1, new Player(1, null, null));

            queueMessageHelper = new QueuedMessageHelper<object, MessageType>();
        }

        [Test]
        public void HandlesEmptyMessageQueueCorrectly()
        {
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Anything)).Return(null);

            npController.Process(1);

            stubIncomingMessageQueue.AssertWasCalled(x => x.ReadMessage(MessageType.PlayerData));
        }

        [Test]
        public void UpdatesPositionBasedOnMessage()
        {
            Player receivedPlayer = new Player(1, null, null);
            receivedPlayer.Position = new Vector2(100, 200);
            queueMessageHelper.QueuedMessages.Enqueue(receivedPlayer);
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.PlayerData))).Do(queueMessageHelper.GetNextQueuedMessage);
            npController.Players[1].Position = Vector2.Zero;

            npController.Process(1);

            Assert.AreEqual(new Vector2(100, 200), npController.Players[1].Position);
        }

        [Test]
        public void UpdatesAllPlayersFromQueue()
        {
            Player receivedPlayer1 = new Player(1, null, null);
            Player receivedPlayer2 = new Player(2, null, null);
            receivedPlayer1.Position = new Vector2(100, 100);
            receivedPlayer2.Position = new Vector2(-100, -100);
            queueMessageHelper.QueuedMessages.Enqueue(receivedPlayer1);
            queueMessageHelper.QueuedMessages.Enqueue(receivedPlayer2);
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Anything)).Do(queueMessageHelper.GetNextQueuedMessage);
            npController.Players.Add(2, new Player(2, null, null));
            npController.Players[1].Position = Vector2.Zero;
            npController.Players[2].Position = Vector2.Zero;

            npController.Process(1);

            Assert.AreEqual(new Vector2(100, 100), npController.Players[1].Position);
            Assert.AreEqual(new Vector2(-100, -100), npController.Players[2].Position);
        }

        [Test]
        public void DoesntCrashOnUnknownPlayer()
        {
            Player receivedPlayer1 = new Player(1, null, null);
            Player receivedPlayer2 = new Player(2, null, null);
            receivedPlayer1.Position = new Vector2(100, 100);
            receivedPlayer2.Position = new Vector2(-100, -100);
            queueMessageHelper.QueuedMessages.Enqueue(receivedPlayer1);
            queueMessageHelper.QueuedMessages.Enqueue(receivedPlayer2);
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Anything)).Do(queueMessageHelper.GetNextQueuedMessage);
            npController.Players[1].Position = Vector2.Zero;

            npController.Process(1);

            Assert.AreEqual(new Vector2(100, 100), npController.Players[1].Position);
        }
    }
}
