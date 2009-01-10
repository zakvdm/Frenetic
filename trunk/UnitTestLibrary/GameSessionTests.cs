using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameSessionTests
    {
        /*
        [Test]
        public void CanConstructGameSession()
        {
            GameSession gameSession = new GameSession();
            Assert.IsNotNull(gameSession);
        }

        [Test]
        public void GameSessionCallsGenerateOnEachView()
        {
            var stubView1 = MockRepository.GenerateStub<IView>();
            var stubView2 = MockRepository.GenerateStub<IView>();
            GameSession gameSession = new GameSession();

            gameSession.Views.Add(stubView1);
            gameSession.Views.Add(stubView2);
            
            stubView1.Stub(x => x.Generate()).Repeat.Once();
            stubView2.Stub(x => x.Generate()).Repeat.Once();
            
            gameSession.Update();

            stubView1.AssertWasCalled(x => x.Generate());
            stubView2.AssertWasCalled(x => x.Generate());
        }

        [Test]
        public void GameSessionCallsProcessOnEachController()
        {
            var stubController1 = MockRepository.GenerateStub<IController>();
            var stubController2 = MockRepository.GenerateStub<IController>();
            GameSession gameSession = new GameSession();

            gameSession.Controllers.Add(stubController1);
            gameSession.Controllers.Add(stubController2);

            stubController1.Stub(x => x.Process()).Repeat.Once();
            stubController2.Stub(x => x.Process()).Repeat.Once();

            gameSession.Update();

            stubController1.AssertWasCalled(x => x.Process());
            stubController2.AssertWasCalled(x => x.Process());
        }
        */
    }
}
