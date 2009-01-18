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
            player.Position = new Vector2(1, 1);
            PlayerView playerView = new PlayerView(player, stubSpriteBatch, stubTexture);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin());
            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture), Arg<Vector2>.Is.Equal(new Vector2(1, 1)),
                Arg<Rectangle>.Is.Equal(null), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f),
                Arg<Vector2>.Is.Equal(new Vector2(stubTexture.Width / 2f, stubTexture.Height / 2f)),
                Arg<Vector2>.Is.Equal(new Vector2(1, 1)), Arg<SpriteEffects>.Is.Equal(SpriteEffects.None), Arg<float>.Is.Equal(0f)));
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }
    }
}
