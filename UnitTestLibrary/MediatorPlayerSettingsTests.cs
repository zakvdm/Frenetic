using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MediatorPlayerSettingsTests
    {
        [Test]
        public void PlayerNamePropertyRegisteredAndWorks()
        {
            Mediator mediator = new Mediator();
            PlayerSettings playerSettings = new PlayerSettings();

            new MediatorPlayerSettingsController(playerSettings, mediator);

            mediator.Do(MediatorPlayerSettingsController.PlayerNameString, "B1FF");

            Assert.AreEqual("B1FF", playerSettings.Name);
            Assert.AreEqual("B1FF", mediator.Get(MediatorPlayerSettingsController.PlayerNameString));
        }

        // COLOR:
        [Test]
        public void PlayerColorPropertyRegisteredAndWorks()
        {
            Mediator mediator = new Mediator();
            PlayerSettings playerSettings = new PlayerSettings();

            new MediatorPlayerSettingsController(playerSettings, mediator);

            mediator.Do(MediatorPlayerSettingsController.PlayerColorString, "100 200 10"); // NOTE: This test won't work if these values are the default color!

            Assert.AreEqual(100, playerSettings.Color.R);
            Assert.AreEqual(200, playerSettings.Color.G);
            Assert.AreEqual(10, playerSettings.Color.B);
            Assert.AreEqual("100 200 10", mediator.Get(MediatorPlayerSettingsController.PlayerColorString));
        }
        [Test]
        public void ColorNotCorruptedByInvalidInput()
        {
            Mediator mediator = new Mediator();
            PlayerSettings playerSettings = new PlayerSettings();
            Color tmp = playerSettings.Color;
            tmp.R = 10;
            tmp.G = 20;
            tmp.B = 30;
            playerSettings.Color = tmp;

            new MediatorPlayerSettingsController(playerSettings, mediator);

            mediator.Do(MediatorPlayerSettingsController.PlayerColorString, "100 ARG! 10");

            Assert.AreEqual(10, playerSettings.Color.R);
            Assert.AreEqual(20, playerSettings.Color.G);
            Assert.AreEqual(30, playerSettings.Color.B);
        }
    }
}
