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
        public void UpdateCallsGameSessionControllerProcessCorrectly()
        {
            var stubGSC = MockRepository.GenerateStub<IController>();
            GameplayScreen gpScreen = new GameplayScreen(stubGSC);

            gpScreen.Update(new Microsoft.Xna.Framework.GameTime(), false, false);

            stubGSC.AssertWasCalled(x => x.Process());
        }

        [Test]
        public void UpdateCallsGameSessionControllerProcessCorrectlyOnClientAndServer()
        {
            var stubGSCserver = MockRepository.GenerateStub<IController>();
            var stubGSCclient = MockRepository.GenerateStub<IController>();
            GameplayScreen gpScreen = new GameplayScreen(stubGSCserver, stubGSCclient);

            gpScreen.Update(new Microsoft.Xna.Framework.GameTime(), false, false);

            stubGSCserver.AssertWasCalled(x => x.Process());
            stubGSCclient.AssertWasCalled(x => x.Process());
        }
    }
}
