using System;

using Frenetic.Level;

using Autofac.Builder;
using Autofac;

using NUnit.Framework;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Frenetic.Physics;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LevelPieceTests
    {
        [Test]
        public void ReturnsCorrectEdgeInformation()
        {
            LevelPiece piece = LevelPieceHelper.MakeLevelPiece(new Vector2(100, 200), new Vector2(100, 50));

            Assert.AreEqual(50, piece.LeftEdge);
            Assert.AreEqual(150, piece.RightEdge);
            Assert.AreEqual(175, piece.TopEdge);
            Assert.AreEqual(225, piece.BottomEdge);
        }


    }

    public static class LevelPieceHelper
    {
        public static LevelPiece MakeLevelPiece(Vector2 position, Vector2 size)
        {
            var physics = MockRepository.GenerateStub<IPhysicsComponent>();
            physics.Position = position;
            physics.Size = size;
            return new LevelPiece(physics);
        }

    }
}
