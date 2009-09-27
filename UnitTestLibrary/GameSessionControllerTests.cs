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
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameSessionControllerTests
    {
        [Test]
        public void CallsProcessOnAllGameSessionControllersAndViews()
        {
            var gameSession = new GameSession();
            GameSessionController gsc = new GameSessionController(gameSession);

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
            stubView1.AssertWasNotCalled(x => x.Generate(1f));
            stubView2.AssertWasNotCalled(x => x.Generate(1f));
        }
        
    }
}
