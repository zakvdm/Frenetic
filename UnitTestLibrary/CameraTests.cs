using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class CameraTests
    {
        [Test]
        public void CameraHasScreenWidthAndHeight()
        {
            Camera camera = new Camera(null, new Vector2(100, 200));

            Assert.AreEqual(100, camera.ScreenWidth);
            Assert.AreEqual(200, camera.ScreenHeight);
        }
        [Test]
        public void CameraPositionBasedOnPlayerPosition()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.Position = new Vector2(100, 200);

            var camera = new Camera(player, new Vector2(0, 0));
            Assert.AreEqual(new Vector2(100, 200), camera.Position);

            player.Position = new Vector2(1, 2);
            Assert.AreEqual(new Vector2(1, 2), camera.Position);
        }

        [Test]
        public void ReturnsCorrectTranslationMatrix()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.Position = new Vector2(100, 200);

            var camera = new Camera(player, new Vector2(0, 0));

            Matrix translationMatrix = Matrix.CreateTranslation(-100, -200, 0.0f);

            Assert.AreEqual(translationMatrix, camera.TranslationMatrix);
        }

        [Test]
        public void TranslationMatrixTakesScreenSizeIntoAccount()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.Position = new Vector2(100, 200);

            var camera = new Camera(player, new Vector2(400, 500));

            Matrix translationMatrix = Matrix.CreateTranslation(-100 + 200, -200 + 250, 0.0f);

            Assert.AreEqual(translationMatrix, camera.TranslationMatrix);
        }

        [Test]
        public void CanConvertCameraPointToWorldCoordinates()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.Position = new Vector2(100, 200);

            var camera = new Camera(player, new Vector2(400, 500));



            Matrix translationMatrix = Matrix.CreateTranslation(-100 + 200, -200 + 250, 0.0f);

            Assert.AreEqual(new Vector2(-50, 50), camera.ConvertToWorldCoordinates(new Vector2(50, 100)));
        }

    }
}
