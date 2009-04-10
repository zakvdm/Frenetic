using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network.Lidgren;
using Lidgren.Network;
using Frenetic.Physics;
using Microsoft.Xna.Framework;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameSessionControllerTests
    {
        QueuedMessageHelper<object, MessageType> queueMH;
        [SetUp]
        public void SetUp()
        {
            queueMH = new QueuedMessageHelper<object, MessageType>();
        }
        [Test]
        public void ConstructingGameSessionControllerCreatesNetworkPlayerController()
        {
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, null, null, null, null, null);

            Assert.AreEqual(1, gameSession.Controllers.Count);
            Assert.IsInstanceOfType(typeof(NetworkPlayerController), gameSession.Controllers[0]);
        }
        
        [Test]
        public void CallsProcessOnAllGameSessionControllersAndViews()
        {
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, MockRepository.GenerateStub<IIncomingMessageQueue>(), null, null, null, null);

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
            //var stubNS = MockRepository.GenerateStub<INetworkSession>();
            PlayerView pv = new PlayerView(null, null, null, null);
            var stubPlayer = MockRepository.GenerateStub<IPlayer>();
            bool playerFactoryWasUsedCorrectly = false;
            bool playerViewFactoryWasUsedCorrectly = false;
            Player.Factory playerFactory = x => { if (x == 100) playerFactoryWasUsedCorrectly = true; return stubPlayer; };
            PlayerView.Factory playerViewFactory = x => { if (x == stubPlayer) playerViewFactoryWasUsedCorrectly = true; return pv; };
            var stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, null, playerFactory, playerViewFactory, null, null, null, false);
            queueMH.QueuedMessages.Enqueue(100);
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            Assert.IsTrue(playerFactoryWasUsedCorrectly);
            Assert.IsTrue(playerViewFactoryWasUsedCorrectly);
            Assert.IsTrue(((NetworkPlayerController)gameSession.Controllers[0]).Players.ContainsKey(100));
            Assert.AreEqual(1, gameSession.Views.FindAll(x => x.GetType() == typeof(PlayerView)).Count);
            Assert.IsTrue(gameSession.Views.Contains(pv));
        }
        
        [Test]
        public void HandlesNewPlayerMessageCorrectlyAsServer()
        {
            bool playerFactoryWasUsedCorrectly = false;
            Player.Factory playerFactory = x => { if (x == 100) playerFactoryWasUsedCorrectly = true; return null; };
            var stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            var stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, stubOutgoingMessageQueue, stubClientStateTracker, playerFactory, null);
            queueMH.QueuedMessages.Enqueue(100);
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            Assert.IsTrue(playerFactoryWasUsedCorrectly);
            Assert.IsTrue(((NetworkPlayerController)gameSession.Controllers[0]).Players.ContainsKey(100));
            Assert.IsInstanceOfType(typeof(NetworkPlayerView), gameSession.Views[0]);

            stubClientStateTracker.AssertWasCalled(x => x.AddNewClient(Arg<int>.Is.Equal(100)));

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
            var stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, stubOutgoingMessageQueue, MockRepository.GenerateStub<IClientStateTracker>(), delegate { return null; }, null);
            queueMH.QueuedMessages.Enqueue(100);
            queueMH.QueuedMessages.Enqueue(200);
            queueMH.QueuedMessages.Enqueue(300);
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

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
            var stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, stubOutgoingMessageQueue, MockRepository.GenerateStub<IClientStateTracker>(), delegate { return null; }, null);
            queueMH.QueuedMessages.Enqueue(100);
            queueMH.QueuedMessages.Enqueue(200);
            queueMH.QueuedMessages.Enqueue(300);
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            stubOutgoingMessageQueue.AssertWasNotCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 300),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<int>.Is.Equal(300)));
        }
        
        [Test]
        public void HandlesSuccessfulJoinMessageCorrectly()
        {
            IPlayer localPlayer = MockRepository.GenerateStub<IPlayer>();
            Client localClient = new Client();
            var stubIncomingMessageQueue = MockRepository.GenerateStub<IIncomingMessageQueue>();
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubIncomingMessageQueue, null, null, null, localPlayer, localClient, null, false);
            queueMH.QueuedMessages.Enqueue(100);
            stubIncomingMessageQueue.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.SuccessfulJoin))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process(1);

            Assert.AreEqual(100, localPlayer.ID);
            Assert.AreEqual(100, localClient.ID);
        }
    }
}
