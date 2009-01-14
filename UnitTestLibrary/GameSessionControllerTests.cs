using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network.Lidgren;
using Lidgren.Network;
using Frenetic.Physics;

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
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            var stubMQ = MockRepository.GenerateStub<MessageQueue>(stubNS);
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubMQ, stubNS, null, null);

            Assert.AreEqual(1, gameSession.Controllers.Count);
            Assert.IsInstanceOfType(typeof(NetworkPlayerController), gameSession.Controllers[0]);
        }

        [Test]
        public void CallsProcessOnAllGameSessionControllersAndViews()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            var stubMQ = MockRepository.GenerateStub<MessageQueue>(stubNS);
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubMQ, stubNS, null, null);

            var stubController1 = MockRepository.GenerateStub<IController>();
            var stubController2 = MockRepository.GenerateStub<IController>();
            var stubView1 = MockRepository.GenerateStub<IView>();
            var stubView2 = MockRepository.GenerateStub<IView>();
            gameSession.Controllers.Add(stubController1);
            gameSession.Controllers.Add(stubController2);
            gameSession.Views.Add(stubView1);
            gameSession.Views.Add(stubView2);

            gsc.Process();

            stubController1.AssertWasCalled(x => x.Process());
            stubController2.AssertWasCalled(x => x.Process());
            stubView1.AssertWasNotCalled(x => x.Generate());
            stubView2.AssertWasNotCalled(x => x.Generate());
        }

        [Test]
        public void HandlesNewPlayerMessageCorrectlyAsClient()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            stubNS.Stub(x => x.IsServer).Return(false);
            PlayerView pv = new PlayerView(null, null, null);
            var stubVF = MockRepository.GenerateStub<IViewFactory>();
            stubVF.Stub(x => x.MakePlayerView(Arg<Player>.Is.Anything)).Return(pv);
            bool playerFactoryWasUsedCorrectly = false;
            // TODO: How can i check that ID was used correctly as factory parameter
            Player.Factory playerFactory = delegate { playerFactoryWasUsedCorrectly = true; return null; };
            var stubMQ = MockRepository.GenerateStub<MessageQueue>(stubNS);
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubMQ, stubNS, stubVF, playerFactory);
            queueMH.QueuedMessages.Enqueue(100);
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process();

            Assert.IsTrue(playerFactoryWasUsedCorrectly);
            Assert.IsTrue(((NetworkPlayerController)gameSession.Controllers[0]).Players.ContainsKey(100));
            Assert.AreEqual(1, gameSession.Views.FindAll(x => x.GetType() == typeof(PlayerView)).Count);
            Assert.IsTrue(gameSession.Views.Contains(pv));
        }
        [Test]
        public void HandlesNewPlayerMessageCorrectlyAsServer()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            stubNS.Stub(x => x.IsServer).Return(true);
            var stubINC = MockRepository.GenerateStub<INetConnection>();
            stubNS.Stub(x => x[100]).Return(stubINC);
            bool playerFactoryWasUsedCorrectly = false;
            // TODO: How can i check that ID was used correctly as factory parameter
            Player.Factory playerFactory = delegate { playerFactoryWasUsedCorrectly = true; return null; };
            var stubMQ = MockRepository.GenerateStub<MessageQueue>(stubNS);
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubMQ, stubNS, null, playerFactory);
            queueMH.QueuedMessages.Enqueue(100);
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process();

            Assert.IsTrue(playerFactoryWasUsedCorrectly);
            Assert.IsTrue(((NetworkPlayerController)gameSession.Controllers[0]).Players.ContainsKey(100));
            Assert.IsInstanceOfType(typeof(NetworkPlayerView), gameSession.Views[0]);

            stubNS.AssertWasCalled(x => x.Send(Arg<Message>.Matches(y => y.Type == MessageType.SuccessfulJoin && (int)y.Data == 100),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableInOrder1),
                Arg<INetConnection>.Is.Equal(stubINC)));
            stubNS.AssertWasCalled(x => x.SendToAll(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 100),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<INetConnection>.Is.Equal(stubINC)));
        }
        [Test]
        public void NotifiesNewClientsOfAllExistentPlayers()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            stubNS.Stub(x => x.IsServer).Return(true);
            var stubINC100 = MockRepository.GenerateStub<INetConnection>();
            var stubINC200 = MockRepository.GenerateStub<INetConnection>();
            var stubINC300 = MockRepository.GenerateStub<INetConnection>();
            stubNS.Stub(x => x[100]).Return(stubINC100);
            stubNS.Stub(x => x[200]).Return(stubINC200);
            stubNS.Stub(x => x[300]).Return(stubINC300);
            var stubMQ = MockRepository.GenerateStub<MessageQueue>(stubNS);
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubMQ, stubNS, null, delegate { return null; });
            queueMH.QueuedMessages.Enqueue(100);
            queueMH.QueuedMessages.Enqueue(200);
            queueMH.QueuedMessages.Enqueue(300);
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process();

            stubNS.AssertWasCalled(x => x.Send(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 100),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<INetConnection>.Is.Equal(stubINC300)));
            stubNS.AssertWasCalled(x => x.Send(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 200),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<INetConnection>.Is.Equal(stubINC300)));
        }
        [Test]
        public void DoesNOTsendNewPlayerMessageToJoiningClient()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            stubNS.Stub(x => x.IsServer).Return(true);
            var stubINC100 = MockRepository.GenerateStub<INetConnection>();
            var stubINC200 = MockRepository.GenerateStub<INetConnection>();
            var stubINC300 = MockRepository.GenerateStub<INetConnection>();
            stubNS.Stub(x => x[100]).Return(stubINC100);
            stubNS.Stub(x => x[200]).Return(stubINC200);
            stubNS.Stub(x => x[300]).Return(stubINC300);
            var stubMQ = MockRepository.GenerateStub<MessageQueue>(stubNS);
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubMQ, stubNS, null, delegate { return null; });
            queueMH.QueuedMessages.Enqueue(100);
            queueMH.QueuedMessages.Enqueue(200);
            queueMH.QueuedMessages.Enqueue(300);
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.NewPlayer))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process();

            stubNS.AssertWasNotCalled(x => x.Send(Arg<Message>.Matches(y => y.Type == MessageType.NewPlayer && (int)y.Data == 300),
                Arg<NetChannel>.Is.Equal(NetChannel.ReliableUnordered),
                Arg<INetConnection>.Is.Equal(stubINC300)));
        }

        [Test]
        public void HandlesSuccessfulJoinMessageCorrectly()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            stubNS.Stub(x => x.IsServer).Return(false);
            PlayerView pv = new PlayerView(null, null, null);
            var stubVF = MockRepository.GenerateStub<IViewFactory>();
            stubVF.Stub(x => x.MakePlayerView(Arg<Player>.Is.Anything)).Return(pv);
            bool playerFactoryWasUsedCorrectly = false;
            // TODO: How can i check that ID was used correctly as factory parameter
            Player.Factory playerFactory = delegate { playerFactoryWasUsedCorrectly = true; return MockRepository.GenerateStub<IPlayer>(); };
            var stubMQ = MockRepository.GenerateStub<MessageQueue>(stubNS);
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession, stubMQ, stubNS, stubVF, playerFactory);
            queueMH.QueuedMessages.Enqueue(100);
            stubMQ.Stub(x => x.ReadMessage(Arg<MessageType>.Is.Equal(MessageType.SuccessfulJoin))).Do(queueMH.GetNextQueuedMessage);

            gsc.Process();

            Assert.IsTrue(playerFactoryWasUsedCorrectly);
            Assert.IsInstanceOfType(typeof(KeyboardPlayerController), gameSession.Controllers[1]);
            Assert.AreEqual(1, gameSession.Views.FindAll(x => x.GetType() == typeof(NetworkPlayerView)).Count);
            Assert.AreEqual(1, gameSession.Views.FindAll(x => x.GetType() == typeof(PlayerView)).Count);
            Assert.IsTrue(gameSession.Views.Contains(pv));
        }
    }
}
