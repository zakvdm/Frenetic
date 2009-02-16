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
        [Test]
        public void CallsDrawWithCorrectParameters()
        {
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            Player player = new Player(1, null, null);
            var camera = new Camera(player, new Vector2(1, 2));
            player.Position = new Vector2(1, 1);
            PlayerView playerView = new PlayerView(player, stubSpriteBatch, stubTexture, camera);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture), Arg<Vector2>.Is.Equal(new Vector2(1, 1)),
                Arg<Rectangle>.Is.Equal(null), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f),
                Arg<Vector2>.Is.Equal(new Vector2(stubTexture.Width / 2f, stubTexture.Height / 2f)),
                Arg<Vector2>.Is.Equal(new Vector2(1, 1)), Arg<SpriteEffects>.Is.Equal(SpriteEffects.None), Arg<float>.Is.Equal(1f)));
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }

        [Test]
        public void UsesCameraCorrectly()
        {
            Player player = new Player();
            Camera camera = new Camera(player, new Vector2(100, 100));
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            PlayerView playerView = new PlayerView(player, stubSpriteBatch, MockRepository.GenerateStub<ITexture>(), camera);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin(Arg<Matrix>.Is.Equal(camera.TranslationMatrix)));
        }
    }
}
