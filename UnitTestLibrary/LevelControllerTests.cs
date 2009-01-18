using System;

using Frenetic.Level;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LevelControllerTests
    {
        [Test]
        public void ProcessOnUnloadedLevelLoadsEmptyLevel()
        {
            var stubLevelLoader = MockRepository.GenerateStub<ILevelLoader>();
            Level level = new Level(stubLevelLoader);

            Assert.IsFalse(level.Loaded);

            LevelController levelController = new LevelController(level);

            levelController.Process(1);

            Assert.IsTrue(level.Loaded);
            stubLevelLoader.AssertWasCalled(x => x.LoadEmptyLevel(Arg<List<LevelPiece>>.Is.Equal(level.Pieces), Arg<int>.Is.Anything, Arg<int>.Is.Anything));
        }
    }
}
