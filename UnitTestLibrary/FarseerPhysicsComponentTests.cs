using System;

using NUnit.Framework;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Collisions;
using Frenetic.Physics;
using Microsoft.Xna.Framework;

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

        [Test]
        public void CanSetAndGetPosition()
        {
            PhysicsSimulator physicsSimulator = new PhysicsSimulator();
            Body body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 10, 10, 100);
            Geom geom = GeomFactory.Instance.CreateRectangleGeom(body, 10, 10);
            FarseerPhysicsComponent farseerPComponent = new FarseerPhysicsComponent(body, geom);

            Assert.AreEqual(new Vector2(0, 0), farseerPComponent.Position);
            farseerPComponent.Position = new Vector2(100, 200);

            Assert.AreEqual(new Vector2(100, 200), farseerPComponent.Position);
        }

        [Test]
        public void CanSetAndGetIsStatic()
        {
            PhysicsSimulator physicsSimulator = new PhysicsSimulator();
            Body body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 10, 10, 100);
            Geom geom = GeomFactory.Instance.CreateRectangleGeom(body, 10, 10);
            FarseerPhysicsComponent farseerPComponent = new FarseerPhysicsComponent(body, geom);

            Assert.IsFalse(farseerPComponent.IsStatic);
            farseerPComponent.IsStatic = true;

            Assert.IsTrue(farseerPComponent.IsStatic);
        }

        [Test]
        public void CanSetAndGetSize()
        {
            PhysicsSimulator physicsSimulator = new PhysicsSimulator();
            Body body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 10, 10, 100);
            Geom geom = GeomFactory.Instance.CreateRectangleGeom(body, 10, 10);
            FarseerPhysicsComponent farseerPComponent = new FarseerPhysicsComponent(body, geom);

            Assert.AreEqual(new Vector2(10, 10), farseerPComponent.Size);
            farseerPComponent.Size = new Vector2(100, 200);

            Assert.AreEqual(new Vector2(100, 200), farseerPComponent.Size);
        }
    }
}
