using System;

using Frenetic.Physics;

using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class WorldBoundaryColliderTests
    {
        [Test]
        public void CanConstruct()
        {
            WorldBoundaryCollider wbc = new WorldBoundaryCollider(100, 200);
            Assert.IsNotNull(wbc);
        }

        [Test]
        public void MovesPositionBackIntoBoundary()
        {
            WorldBoundaryCollider wbc = new WorldBoundaryCollider(100, 100);

            Vector2 pos = new Vector2(105, 30);

            pos = wbc.MoveWithinBoundary(pos);

            Assert.AreEqual(new Vector2(100, 30), pos);
        }

        [Test]
        public void MovesIntoBoundaryForNegativeValues()
        {
            WorldBoundaryCollider wbc = new WorldBoundaryCollider(100, 100);

            Vector2 pos = new Vector2(-20, -50);

            pos = wbc.MoveWithinBoundary(pos);

            Assert.AreEqual(new Vector2(0, 0), pos);
        }
    }
}
