using System;

using Frenetic.Level;

using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Physics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class DumbLevelLoaderTests
    {
        [Test]
        public void RequiresALevelPieceFactory()
        {
            DumbLevelLoader dumbLevelLoader = new DumbLevelLoader(MakeLevelPiece);
            Assert.IsNotNull(dumbLevelLoader);
        }

        private LevelPiece MakeLevelPiece(Vector2 position, Vector2 size)
        {
            return new LevelPiece(position, size, MockRepository.GenerateStub<IPhysicsComponent>());
        }

        [Test]
        public void LoadEmptyLevelTakesSizeAndFillsInEdges()
        {
            DumbLevelLoader dumbLevelLoader = new DumbLevelLoader(MakeLevelPiece);
            
            List<LevelPiece> levelPieces = new List<LevelPiece>();

            dumbLevelLoader.LoadEmptyLevel(levelPieces, 400, 600);

            // level built clockwise starting on left
            Assert.AreEqual(new Vector2(10, 300), levelPieces[0].Position);
            Assert.AreEqual(new Vector2(20, 600), levelPieces[0].Size);

            Assert.AreEqual(new Vector2(200, 10), levelPieces[1].Position);
            Assert.AreEqual(new Vector2(400, 20), levelPieces[1].Size);

            Assert.AreEqual(new Vector2(390, 300), levelPieces[2].Position);
            Assert.AreEqual(new Vector2(20, 600), levelPieces[2].Size);

            Assert.AreEqual(new Vector2(200, 590), levelPieces[3].Position);
            Assert.AreEqual(new Vector2(400, 20), levelPieces[3].Size);
        }
    }
}
