using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class QuitterTests
    {
        [Test]
        public void SavesSettingsBeforeQuit()
        {
            ISettingsPersister stubSettingsSaver = MockRepository.GenerateStub<ISettingsPersister>();
            Quitter quitter = new Quitter(stubSettingsSaver, MockRepository.GenerateStub<IGame>());

            quitter.Quit();

            stubSettingsSaver.AssertWasCalled(me => me.SaveSettings());
        }

        [Test]
        public void QuitsTheGame()
        {
            IGame stubGame = MockRepository.GenerateStub<IGame>();
            Quitter quitter = new Quitter(MockRepository.GenerateStub<ISettingsPersister>(), stubGame);

            quitter.Quit();

            stubGame.AssertWasCalled(me => me.Exit());
        }
    }
}
