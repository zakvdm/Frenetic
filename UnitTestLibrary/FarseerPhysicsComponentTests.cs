using System;

using NUnit.Framework;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Collisions;
using Frenetic.Physics;
using Microsoft.Xna.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class FarseerPhysicsComponentTests
    {
        PhysicsSimulator simulator = new PhysicsSimulator();
        Body body;
        Geom geom;
        FarseerPhysicsComponent farseerPComponent;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            body = BodyFactory.Instance.CreateRectangleBody(simulator, 10, 10, 100);
            geom = GeomFactory.Instance.CreateRectangleGeom(body, 10, 10);
            farseerPComponent = new FarseerPhysicsComponent(body, geom);
        }

        [Test]
        public void AddsPhysicsComponentAsTagOnGeom()
        {
            Assert.AreEqual(farseerPComponent, geom.Tag as IPhysicsComponent);
        }

        [Test]
        public void CanGetAndSetPosition()
        {
            farseerPComponent.Position = new Vector2(100, 200);

            Assert.AreEqual(new Vector2(100, 200), farseerPComponent.Position);
        }

        [Test]
        public void CanSetAndGetIsStatic()
        {
            Assert.IsFalse(farseerPComponent.IsStatic);

            farseerPComponent.IsStatic = true;

            Assert.IsTrue(farseerPComponent.IsStatic);
        }

        [Test]
        public void CanSetAndGetSize()
        {
            Assert.AreEqual(new Vector2(10, 10), farseerPComponent.Size);
        }

        [Test]
        public void CanApplyImpulse()
        {
            farseerPComponent.Position = new Vector2(0, 0);

            farseerPComponent.ApplyImpulse(Vector2.UnitX);
            simulator.Update(1);

            Assert.AreNotEqual(new Vector2(0, 0), body.Position);
        }

        [Test]
        public void CanApplyForce()
        {
            Assert.AreEqual(Vector2.Zero, body.Force);

            farseerPComponent.ApplyForce(Vector2.UnitX);

            Assert.AreEqual(Vector2.UnitX, body.Force);
        }

        [Test]
        public void CanGetAndSetLinearVelocity()
        {
            farseerPComponent.LinearVelocity = new Vector2(3, 5);

            Assert.AreEqual(new Vector2(3, 5), farseerPComponent.LinearVelocity);
        }

        [Test]
        public void RaisesEventWhenShot()
        {
            bool raisedEvent = false;
            farseerPComponent.Shot += () => raisedEvent = true;

            farseerPComponent.OnShot();

            Assert.IsTrue(raisedEvent);
        }
    }
}
