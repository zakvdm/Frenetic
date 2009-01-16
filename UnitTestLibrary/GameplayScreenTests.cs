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
            GameplayScreen gpScreen = new GameplayScreen(new GameSessionControllerAndView(null, stubGSC, null));

            gpScreen.Update(new Microsoft.Xna.Framework.GameTime(), false, false);

            stubGSC.AssertWasCalled(x => x.Process(Arg<long>.Is.Anything));
        }

        [Test]
        public void DrawCallsGameSessionViewGenerateCorrectlyOnClient()
        {
            var stubGSV = MockRepository.GenerateStub<IView>();
            GameplayScreen gpScreen = new GameplayScreen(new GameSessionControllerAndView(null, null, stubGSV));

            gpScreen.Draw(new Microsoft.Xna.Framework.GameTime());

            stubGSV.AssertWasCalled(x => x.Generate());
        }

        [Test]
        public void UpdateCallsGameSessionControllerProcessCorrectlyOnClientAndServer()
        {
            var stubGSCserver = MockRepository.GenerateStub<IController>();
            var stubGSCclient = MockRepository.GenerateStub<IController>();
            GameplayScreen gpScreen = new GameplayScreen(new GameSessionControllerAndView(null, stubGSCserver, null),
                                        new GameSessionControllerAndView(null, stubGSCclient, null));

            gpScreen.Update(new Microsoft.Xna.Framework.GameTime(), false, false);

            stubGSCserver.AssertWasCalled(x => x.Process(Arg<long>.Is.Anything));
            stubGSCclient.AssertWasCalled(x => x.Process(Arg<long>.Is.Anything));
        }

        [Test]
        public void DrawCallsGameSessionViewGenerateCorrectlyOnClientAndServer()
        {
            var stubGSVserver = MockRepository.GenerateStub<IView>();
            var stubGSVclient = MockRepository.GenerateStub<IView>();
            GameplayScreen gpScreen = new GameplayScreen(new GameSessionControllerAndView(null, null, stubGSVserver),
                                        new GameSessionControllerAndView(null, null, stubGSVclient));

            gpScreen.Draw(new Microsoft.Xna.Framework.GameTime());

            stubGSVserver.AssertWasCalled(x => x.Generate());
            stubGSVclient.AssertWasCalled(x => x.Generate());
        }
    }
}
