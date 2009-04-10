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

            int halfwidth = DumbLevelLoader.BOUNDARY / 2;
            int width = DumbLevelLoader.BOUNDARY;

            // level built clockwise starting on left
            Assert.AreEqual(new Vector2(-halfwidth, 300), levelPieces[0].Position);
            Assert.AreEqual(new Vector2(width, 600), levelPieces[0].Size);

            Assert.AreEqual(new Vector2(200, -halfwidth), levelPieces[1].Position);
            Assert.AreEqual(new Vector2(400, width), levelPieces[1].Size);

            Assert.AreEqual(new Vector2(400 + halfwidth, 300), levelPieces[2].Position);
            Assert.AreEqual(new Vector2(width, 600), levelPieces[2].Size);

            Assert.AreEqual(new Vector2(200, 600 + halfwidth), levelPieces[3].Position);
            Assert.AreEqual(new Vector2(400, width), levelPieces[3].Size);
        }
    }
}
