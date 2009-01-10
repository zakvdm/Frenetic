using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameSessionFactoryTests
    {
        /*
        [Test]
        public void CanCreateGameSessionFactory()
        {
            var stubNSF = MockRepository.GenerateStub<INetworkSessionFactory>();
            GameSessionFactory gsFactory = new GameSessionFactory(stubNSF);

            Assert.IsNotNull(gsFactory);
        }

        [Test]
        public void CanCreateAServerGameSession()
        {
            var stubNSF = MockRepository.GenerateStub<INetworkSessionFactory>();
            GameSessionFactory gsFactory = new GameSessionFactory(stubNSF);
            stubNSF.Stub(x => x.MakeServerNetworkSession()).Return(MockRepository.GenerateStub<INetworkSession>());
            IGameSession gs = gsFactory.MakeServerGameSession();

            Assert.IsNotNull(gs);
            Assert.AreEqual(1, gs.Controllers.Count);
            Assert.IsInstanceOfType(typeof(NetworkPlayerController), gs.Controllers[0]);
            stubNSF.AssertWasCalled(x => x.MakeServerNetworkSession());
        }

        [Test]
        public void CanCreateAClientGameSession()
        {
            var stubNSF = MockRepository.GenerateStub<INetworkSessionFactory>();
            GameSessionFactory gsFactory = new GameSessionFactory(stubNSF);
            stubNSF.Stub(x => x.MakeClientNetworkSession()).Return(MockRepository.GenerateStub<INetworkSession>());
            IGameSession gs = gsFactory.MakeClientGameSession();

            Assert.IsNotNull(gs);
            Assert.AreEqual(1, gs.Controllers.Count);
            Assert.IsInstanceOfType(typeof(NetworkPlayerController), gs.Controllers[0]);
            stubNSF.AssertWasCalled(x => x.MakeClientNetworkSession());
        }

        [Test]
        public void CreateServerGameSessionAddsANetworkWorldView()
        {
            var stubNSF = MockRepository.GenerateStub<INetworkSessionFactory>();
            GameSessionFactory gsFactory = new GameSessionFactory(stubNSF);
            stubNSF.Stub(x => x.MakeServerNetworkSession()).Return(MockRepository.GenerateStub<INetworkSession>());
            
            IGameSession gs = gsFactory.MakeServerGameSession();

            Assert.AreEqual(1, gs.Views.Count);
            Assert.IsInstanceOfType(typeof(NetworkWorldView), gs.Views[0]);
        }
         */
    }
}
