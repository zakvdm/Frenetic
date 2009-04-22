using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network.Lidgren;
using Lidgren.Network;
using Frenetic.Physics;
using Microsoft.Xna.Framework;
using Frenetic.Network;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameSessionControllerTests
    {
        QueuedMessageHelper<Message, MessageType> queueMH;
        IIncomingMessageQueue stubIncomingMessageQueue;
        IOutgoingMessageQueue stubOutgoingMessageQueue;

        [SetUp]
        public void SetUp()
        {
            queueMH = new QueuedMessageHelper<Message, MessageType>();
            stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
        }
        
        [Test]
        public void CallsProcessOnAllGameSessionControllersAndViews()
        {
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, null, null);

            var stubController1 = MockRepository.GenerateStub<IController>();
            var stubController2 = MockRepository.GenerateStub<IController>();
            var stubView1 = MockRepository.GenerateStub<IView>();
            var stubView2 = MockRepository.GenerateStub<IView>();
            gameSession.Controllers.Add(stubController1);
            gameSession.Controllers.Add(stubController2);
            gameSession.Views.Add(stubView1);
            gameSession.Views.Add(stubView2);

            gsc.Process(1);

            stubController1.AssertWasCalled(x => x.Process(1));
            stubController2.AssertWasCalled(x => x.Process(1));
            stubView1.AssertWasNotCalled(x => x.Generate());
            stubView2.AssertWasNotCalled(x => x.Generate());
        }
        
        [Test]
        public void HandlesNewPlayerMessageCorrectlyAsClient()
        {
            var stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            var stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubClientStateTracker.Stub(me => me[100]).Return(new Client(stubPlayer, null));
            PlayerView pv = new PlayerView(null, null, null, null, null);
            bool playerViewFactoryWasUsedCorrectly = false;
            PlayerView.Factory playerViewFactory = x => { if (x == stubPlayer) playerViewFactoryWasUsedCorrectly = true; return pv; };
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, null, stubClientStateTracker, playerViewFactory, null, false);
            queueMH.QueuedMessages.Enqueue(new Message() { Data=100 });
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            stubClientStateTracker.AssertWasCalled(me => me.AddNewClient(Arg<int>.Is.Equal(100)));
            Assert.IsTrue(playerViewFactoryWasUsedCorrectly);
            Assert.AreEqual(1, gameSession.Views.FindAll(x => x.GetType() == typeof(PlayerView)).Count);
            Assert.IsTrue(gameSession.Views.Contains(pv));
        }
        
        [Test]
        public void HandlesNewPlayerMessageCorrectlyAsServer()
        {
            ClientStateTracker clientStateTracker = new ClientStateTracker(MockRepository.GenerateStub<ISnapCounter>(), () => new Client(new Player(null, null), null));
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, stubOutgoingMessageQueue, clientStateTracker);
            queueMH.QueuedMessages.Enqueue(new Message() { Data=100 });
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            Assert.IsNotNull(clientStateTracker[100]);
            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.SuccessfulJoin && (int)y.Data == 100),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableInOrder1),
                Arg<int>.Is.Equal(100)));

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteForAllExcept(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 100),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<int>.Is.Equal(100)));
        }
        
        [Test]
        public void NotifiesNewClientsOfAllExistentPlayers()
        {
            var clientStateTracker = new ClientStateTracker(MockRepository.GenerateStub<ISnapCounter>(), () => new Client(new Player(null, null), null));
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, stubOutgoingMessageQueue, clientStateTracker);
            queueMH.QueuedMessages.Enqueue(new Message() { Data=100 });
            queueMH.QueuedMessages.Enqueue(new Message() { Data=200 });
            queueMH.QueuedMessages.Enqueue(new Message() { Data=300 });
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 100),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<int>.Is.Equal(300)));
            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 200),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<int>.Is.Equal(300)));
        }
        
        [Test]
        public void DoesNOTsendNewPlayerMessageToJoiningClient()
        {
            ClientStateTracker clientStateTracker = new ClientStateTracker(MockRepository.GenerateStub<ISnapCounter>(), () => new Client(new Player(null, null), new PlayerSettings()));
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, stubOutgoingMessageQueue, clientStateTracker);
            queueMH.QueuedMessages.Enqueue(new Message() { Data = 100 });
            queueMH.QueuedMessages.Enqueue(new Message() { Data = 200 });
            queueMH.QueuedMessages.Enqueue(new Message() { Data = 300 });
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            stubOutgoingMessageQueue.AssertWasNotCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 300),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<int>.Is.Equal(300)));
        }
        
        [Test]
        public void HandlesSuccessfulJoinMessageCorrectly()
        {
            IPlayer localPlayer = MockRepository.GenerateStub<IPlayer>();
            LocalClient localClient = new LocalClient(null, null);
            var stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, null, null, null, localClient, false);
            queueMH.QueuedMessages.Enqueue(new Message() { Data=100 });
            stubIncomingMessageQueue.Stub(x => x.ReadWholeMessage(Arg<MessageType>.Is.Equal(MessageType.SuccessfulJoin))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            Assert.AreEqual(100, localClient.ID);
        }
    }
}
