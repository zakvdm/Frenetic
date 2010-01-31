using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Weapons;
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
        public void ShootCreatesNewShot()
        {
            rocketLauncher.Shoot(Vector2.One, Vector2.UnitX);

            Assert.AreEqual(1, rocketLauncher.Shots.Count);
            Assert.AreEqual(new Shot(Vector2.One, Vector2.UnitX), rocketLauncher.Shots[0]);
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
