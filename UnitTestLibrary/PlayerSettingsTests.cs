using System;
using NUnit.Framework;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerSettingsTests
    {
        LocalPlayerSettings playerSettings;
        [SetUp]
        public void SetUp()
        {
            playerSettings = new LocalPlayerSettings();
        }

        [Test]
        public void ChangingASettingDirtiesState()
        {
            playerSettings.Clean();

            playerSettings.Name = "new name";

            Assert.IsTrue(playerSettings.IsDirty);
        }

        [Test]
        public void CanCleanState()
        {
            playerSettings.Color = new Microsoft.Xna.Framework.Graphics.Color();

            playerSettings.Clean();

            Assert.IsFalse(playerSettings.IsDirty);
        }

        [Test]
        public void DiffWorks()
        {
            playerSettings.Texture = Frenetic.Graphics.PlayerTexture.Blank;

            var diffedSettings = playerSettings.GetDiff();

            Assert.AreEqual(playerSettings.Texture, diffedSettings.Texture);
            Assert.AreEqual(playerSettings.Name, diffedSettings.Name);
        }
    }
}
