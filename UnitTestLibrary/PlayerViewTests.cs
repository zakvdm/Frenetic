using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerViewTests
    {
        PlayerSettings _settings;
        [SetUp]
        public void SetUp()
        {
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            _settings = new PlayerSettings(stubTexture);
        }

        [Test]
        public void CallsDrawWithCorrectParameters()
        {
            _settings.Texture.Stub(x => x.Width).Return(100);
            _settings.Texture.Stub(x => x.Height).Return(200);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            Player player = new Player(1, null, null);
            var camera = new Camera(player, new Vector2(1, 2));
            player.Position = new Vector2(1, 1);
            PlayerView playerView = new PlayerView(player, _settings, stubSpriteBatch, camera);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(_settings.Texture), Arg<Vector2>.Is.Equal(new Vector2(1, 1)),
                Arg<Rectangle>.Is.Equal(null), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f),
                Arg<Vector2>.Is.Equal(new Vector2(100 / 2f, 200 / 2f)),
                Arg<Vector2>.Is.Equal(new Vector2(1, 1)), Arg<SpriteEffects>.Is.Equal(SpriteEffects.None), Arg<float>.Is.Equal(1f)));
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }

        [Test]
        public void UsesCameraCorrectly()
        {
            Player player = new Player();
            Camera camera = new Camera(player, new Vector2(100, 100));
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            PlayerView playerView = new PlayerView(player, _settings, stubSpriteBatch, camera);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin(Arg<Matrix>.Is.Equal(camera.TranslationMatrix)));
        }
    }
}
