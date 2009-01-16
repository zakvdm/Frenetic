using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetworkPlayerControllerTests
    {
        INetworkSession stubNS;
        MessageQueue stubMQ;
        NetworkPlayerController npController;
        QueuedMessageHelper<object, MessageType> queueMH;
        [SetUp]
        public void SetUp()
        {
            stubNS = MockRepository.GenerateStub<INetworkSession>();
            stubMQ = MockRepository.GenerateStub<MessageQueue>(stubNS);
            npController = new NetworkPlayerController(stubMQ);
            npController.Players.Add(1, new Player(1, null, null));

            queueMH = new QueuedMessageHelper<object, MessageType>();
        }

        [Test]
        public void HandlesEmptyMessageQueueCorrectly()
        {
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Anything)).Return(null);

            npController.Process(1);

            stubMQ.AssertWasCalled(x => x.ReadMessage(MessageType.PlayerData));
        }

        [Test]
        public void UpdatesPositionBasedOnMessage()
        {
            Player receivedPlayer = new Player(1, null, null);
            receivedPlayer.Position = new Vector2(100, 200);
            queueMH.QueuedMessages.Enqueue(receivedPlayer);
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.PlayerData))).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(Vector2.Zero, npController.Players[1].Position);

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
            queueMH.QueuedMessages.Enqueue(receivedPlayer1);
            queueMH.QueuedMessages.Enqueue(receivedPlayer2);
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Anything)).Do(queueMH.GetNextQueuedMessage);
            npController.Players.Add(2, new Player(2, null, null));

            Assert.AreEqual(Vector2.Zero, npController.Players[1].Position);
            Assert.AreEqual(Vector2.Zero, npController.Players[2].Position);
            
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
            queueMH.QueuedMessages.Enqueue(receivedPlayer1);
            queueMH.QueuedMessages.Enqueue(receivedPlayer2);
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Anything)).Do(queueMH.GetNextQueuedMessage);

            Assert.AreEqual(Vector2.Zero, npController.Players[1].Position);

            npController.Process(1);

            Assert.AreEqual(new Vector2(100, 100), npController.Players[1].Position);
        }
    }
}
