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
            WorldBoundaryCollider wbc = new WorldBoundaryCollider(new PhysicsValues());
            Assert.IsNotNull(wbc);
        }

        [Test]
        public void MovesPositionBackIntoBoundary()
        {
            PhysicsValues pv = new PhysicsValues();
            pv.Width = 100;
            pv.Height = 100;
            WorldBoundaryCollider wbc = new WorldBoundaryCollider(pv);

            Vector2 pos = new Vector2(105, 30);

            pos = wbc.MoveWithinBoundary(pos);

            Assert.AreEqual(new Vector2(100, 30), pos);
        }

        [Test]
        public void MovesIntoBoundaryForNegativeValues()
        {
            PhysicsValues pv = new PhysicsValues();
            pv.Width = 100;
            pv.Height = 100;
            WorldBoundaryCollider wbc = new WorldBoundaryCollider(pv);

            Vector2 pos = new Vector2(-20, -50);

            pos = wbc.MoveWithinBoundary(pos);

            Assert.AreEqual(new Vector2(0, 0), pos);
        }
    }
}
