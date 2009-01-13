using System;

using Frenetic;
using Frenetic.Physics;

using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class VerletIntegratorTests
    {
        [Test]
        public void CanConstruct()
        {
            PhysicsValues values = new PhysicsValues();
            VerletIntegrator vi = new VerletIntegrator(values);
            Assert.IsNotNull(vi);
        }

        [Test]
        public void IntegratesCorrectlyWhenOldPositionEqualsPosition()
        {
            PhysicsValues values = new PhysicsValues();
            values.Gravity = 1;
            VerletIntegrator vi = new VerletIntegrator(values);
            vi.LastPosition = new Vector2(100, 200);
            Vector2 pos = new Vector2(100, 200);

            pos = vi.Integrate(pos);

            Assert.AreEqual(201, pos.Y);
            Assert.AreEqual(100, pos.X);
            Assert.AreEqual(200, vi.LastPosition.Y);
            Assert.AreEqual(100, vi.LastPosition.X);
        }

        [Test]
        public void IntegratesCorrectlyWithDrag()
        {
            PhysicsValues values = new PhysicsValues();
            values.Gravity = 10;
            values.Drag = 0.5f;
            VerletIntegrator vi = new VerletIntegrator(values);
            vi.LastPosition = new Vector2(50, 100);
            Vector2 pos = new Vector2(100, 200);

            pos = vi.Integrate(pos);

            Assert.AreEqual(260, pos.Y);
            Assert.AreEqual(125, pos.X);
            Assert.AreEqual(200, vi.LastPosition.Y);
            Assert.AreEqual(100, vi.LastPosition.X);
        }
    }
}
