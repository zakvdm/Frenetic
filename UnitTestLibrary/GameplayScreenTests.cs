using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameplayScreenTests
    {
        [Test]
        public void UpdateCallsGameSessionControllerProcessCorrectlyOnClient()
        {
            var stubGSC = MockRepository.GenerateStub<IController>();
            GameplayScreen gpScreen = new GameplayScreen(new GameSessionControllerAndView(stubGSC, null));

            gpScreen.Update(new Microsoft.Xna.Framework.GameTime(), false, false);

            stubGSC.AssertWasCalled(x => x.Process());
        }

        [Test]
        public void DrawCallsGameSessionViewGenerateCorrectlyOnClient()
        {
            var stubGSV = MockRepository.GenerateStub<IView>();
            GameplayScreen gpScreen = new GameplayScreen(new GameSessionControllerAndView(null, stubGSV));

            gpScreen.Draw(new Microsoft.Xna.Framework.GameTime());

            stubGSV.AssertWasCalled(x => x.Generate());
        }

        [Test]
        public void UpdateCallsGameSessionControllerProcessCorrectlyOnClientAndServer()
        {
            var stubGSCserver = MockRepository.GenerateStub<IController>();
            var stubGSCclient = MockRepository.GenerateStub<IController>();
            GameplayScreen gpScreen = new GameplayScreen(new GameSessionControllerAndView(stubGSCserver, null),
                                        new GameSessionControllerAndView(stubGSCclient, null));

            gpScreen.Update(new Microsoft.Xna.Framework.GameTime(), false, false);

            stubGSCserver.AssertWasCalled(x => x.Process());
            stubGSCclient.AssertWasCalled(x => x.Process());
        }

        [Test]
        public void DrawCallsGameSessionViewGenerateCorrectlyOnClientAndServer()
        {
            var stubGSVserver = MockRepository.GenerateStub<IView>();
            var stubGSVclient = MockRepository.GenerateStub<IView>();
            GameplayScreen gpScreen = new GameplayScreen(new GameSessionControllerAndView(null, stubGSVserver),
                                        new GameSessionControllerAndView(null, stubGSVclient));

            gpScreen.Draw(new Microsoft.Xna.Framework.GameTime());

            stubGSVserver.AssertWasCalled(x => x.Generate());
            stubGSVclient.AssertWasCalled(x => x.Generate());
        }
    }
}
