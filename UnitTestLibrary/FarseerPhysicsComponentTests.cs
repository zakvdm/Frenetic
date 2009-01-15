using System;

using NUnit.Framework;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Collisions;
using Frenetic.Physics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class FarseerPhysicsComponentTests
    {
        [Test]
        public void CanCreate()
        {
            PhysicsSimulator physicsSimulator = new PhysicsSimulator();
            Body body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 10, 10, 100);
            Geom geom = GeomFactory.Instance.CreateRectangleGeom(body, 10, 10);

            FarseerPhysicsComponent farseerPComponent = new FarseerPhysicsComponent(body, geom);
            Assert.IsNotNull(farseerPComponent);
        }
    }
}
