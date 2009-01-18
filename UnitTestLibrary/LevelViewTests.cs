using System;
using NUnit.Framework;
using Frenetic.Level;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LevelViewTests
    {
        [Test]
        public void GenerateDrawsLevelPieceWithCorrectParameters()
        {
            Level level = new Level(null);
            level.Pieces.Add(new LevelPiece(new Vector2(1, 1), new Vector2(100, 100)));
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            LevelView levelView = new LevelView(level, stubSpriteBatch, stubTexture);

            levelView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin());
            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture), Arg<Vector2>.Is.Equal(new Vector2(1, 1)),
                Arg<Rectangle>.Is.Equal(null), Arg<Color>.Is.Equal(level.Pieces[0].Color), Arg<float>.Is.Equal(0f), 
                Arg<Vector2>.Is.Equal(new Vector2(stubTexture.Width / 2f, stubTexture.Height /2f)),
                Arg<Vector2>.Is.Equal(new Vector2(100, 100)), Arg<SpriteEffects>.Is.Equal(SpriteEffects.None), Arg<float>.Is.Equal(0f)));
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }

        [Test]
        public void GenerateDrawsEachLevelPiece()
        {
            Level level = new Level(null);
            level.Pieces.Add(new LevelPiece(new Vector2(1, 1), new Vector2(100, 100)));
            level.Pieces.Add(new LevelPiece(new Vector2(2, 2), new Vector2(50, 50)));
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            LevelView levelView = new LevelView(level, stubSpriteBatch, stubTexture);

            levelView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin());
            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Anything, Arg<Vector2>.Is.Equal(new Vector2(1, 1)),
                Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything, Arg<Vector2>.Is.Anything,
                Arg<Vector2>.Is.Anything, Arg<SpriteEffects>.Is.Anything, Arg<float>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Anything, Arg<Vector2>.Is.Equal(new Vector2(2, 2)),
                Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything, Arg<Vector2>.Is.Anything,
                Arg<Vector2>.Is.Anything, Arg<SpriteEffects>.Is.Anything, Arg<float>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }
    }
}
