using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;

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

            Assert.AreEqual("B1FF", playerSettings.PlayerName);
            Assert.AreEqual("B1FF", mediator.Get(MediatorPlayerSettingsController.PlayerNameString));
        }
    }
}
