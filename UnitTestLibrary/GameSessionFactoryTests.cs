using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Physics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameSessionFactoryTests
    {
        // TODO:  WHAT SHOULD HAPPEN TO THESE TESTS? DO WE TEST OBJECT CONSTRUCTION?
        //      ONE PROBLEM: NOT EVERYTHING IS OBVIOUS FROM CONSTRUCTORS,
        //          EG. WE NEED TO ADD CERTAIN VIEWS/CONTROLLERS TO THE GAME SESSION...
        //              HOW CAN WE BE SURE THIS IS DONE CORRECTLY WITHOUT UNIT TESTS?
        
        [Test]
        public void CanCreateGameSessionFactory()
        {
            var stubNSF = MockRepository.GenerateStub<INetworkSessionFactory>();
            GameSessionFactory gsFactory = new GameSessionFactory(stubNSF, null, null);

            Assert.IsNotNull(gsFactory);
        }

        /*
        [Test]
        public void CreateClientGameSessionAddsAFarseerPhysicsController()
        {
            var stubNSF = MockRepository.GenerateStub<INetworkSessionFactory>();
            GameSessionFactory gsFactory = new GameSessionFactory(stubNSF, null, null);
            stubNSF.Stub(x => x.MakeClientNetworkSession()).Return(MockRepository.GenerateStub<INetworkSession>());

            GameSessionControllerAndView gsCandV = gsFactory.MakeClientGameSession();

            Assert.AreEqual(1, gsCandV.GameSession.Controllers.FindAll(x => x.GetType() == typeof(FarseerPhysicsController)).Count);
        }

        [Test]
        public void CanCreateAServerGameSession()
        {
            var stubNSF = MockRepository.GenerateStub<INetworkSessionFactory>();
            GameSessionFactory gsFactory = new GameSessionFactory(stubNSF, null, null);
            stubNSF.Stub(x => x.MakeServerNetworkSession()).Return(MockRepository.GenerateStub<INetworkSession>());
            GameSessionControllerAndView gsCandV = gsFactory.MakeServerGameSession();

            Assert.IsNotNull(gsCandV.GameSession);
            Assert.IsNotNull(gsCandV.GameSessionController);   // NetworkPlayerController
            Assert.IsNotNull(gsCandV.GameSessionView);
            stubNSF.AssertWasCalled(x => x.MakeServerNetworkSession());
        }

        [Test]
        public void CanCreateAClientGameSession()
        {
            var stubNSF = MockRepository.GenerateStub<INetworkSessionFactory>();
            GameSessionFactory gsFactory = new GameSessionFactory(stubNSF, null, null);
            stubNSF.Stub(x => x.MakeClientNetworkSession()).Return(MockRepository.GenerateStub<INetworkSession>());
            GameSessionControllerAndView gsCandV = gsFactory.MakeClientGameSession();

            Assert.IsNotNull(gsCandV.GameSession);
            Assert.IsNotNull(gsCandV.GameSessionController);   // NetworkPlayerController
            Assert.IsNotNull(gsCandV.GameSessionView);
            stubNSF.AssertWasCalled(x => x.MakeClientNetworkSession());
        }
        */
    }
}
