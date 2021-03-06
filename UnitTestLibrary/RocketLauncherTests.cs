﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Gameplay.Weapons;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Physics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class RocketLauncherTests
    {
        RocketLauncher rocketLauncher;
        [SetUp]
        public void SetUp()
        {
            rocketLauncher = new RocketLauncher(RocketLauncherHelper.MakeRocket);
        }

        [Test]
        public void ShootCreatesNewRocket()
        {
            rocketLauncher.Shoot(Vector2.One, new Vector2(3, 4));

            Assert.AreEqual(1, rocketLauncher.Rockets.Count);
            Assert.AreEqual(Vector2.One + (RocketLauncher.RocketOffset * Vector2.Normalize(new Vector2(3, 4) - Vector2.One)), rocketLauncher.Rockets[0].Position);
            Assert.AreEqual(Rocket.Speed * Vector2.Normalize(new Vector2(3, 4) - Vector2.One), rocketLauncher.Rockets[0].Velocity);
        }

        [Test]
        public void shouldDieWhenRocketHitLevel()
        {
            var stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            Rocket rocket = new Rocket(Vector2.Zero, Vector2.Zero, stubPhysicsComponent);
            Assert.IsTrue(rocket.IsAlive);

            stubPhysicsComponent.Raise(me => me.CollidedWithWorld += null);

            Assert.IsFalse(rocket.IsAlive);
        }

        [Test]
        public void ShouldRemoveDeadRocketsFromList()
        {
            var aliveRocket = new Rocket(Vector2.Zero, Vector2.Zero, MockRepository.GenerateStub<IPhysicsComponent>()) { IsAlive = true };
            var deadRocket = new Rocket(Vector2.Zero, Vector2.Zero, MockRepository.GenerateStub<IPhysicsComponent>()) { IsAlive = false };
            rocketLauncher.Rockets.AddRange(new System.Collections.Generic.List<Rocket>() { aliveRocket, deadRocket });

            rocketLauncher.RemoveDeadProjectiles();

            Assert.Contains(aliveRocket, rocketLauncher.Rockets);
            Assert.AreEqual(1, rocketLauncher.Rockets.Count);
        }
        [Test]
        public void ShouldRemoveDeadRocketPhysicsComponents()
        {
            var mockPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            mockPhysicsComponent.Enabled = true;
            rocketLauncher.Rockets.Add(new Rocket(Vector2.Zero, Vector2.Zero, mockPhysicsComponent) { IsAlive = false });

            rocketLauncher.RemoveDeadProjectiles();

            Assert.IsFalse(mockPhysicsComponent.Enabled);
        }
    }
    public static class RocketLauncherHelper
    {
        public static Rocket MakeRocket(Vector2 position, Vector2 velocity)
        {
            var physics = MockRepository.GenerateStub<IPhysicsComponent>();
            physics.Position = position;
            physics.LinearVelocity = velocity;
            return new Rocket(position, velocity, physics);
        }

    }
}
