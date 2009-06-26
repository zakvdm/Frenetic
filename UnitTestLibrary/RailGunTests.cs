using System;
using NUnit.Framework;
using Frenetic.Weapons;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Physics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class RailGunTests
    {
        IRayCaster stubRayCaster;
        RailGun railGun;
        [SetUp]
        public void SetUp()
        {
            stubRayCaster = MockRepository.GenerateStub<IRayCaster>();
            railGun = new RailGun(stubRayCaster);
        }

        [Test]
        public void CanShoot()
        {
            railGun.Shoot(new Vector2(100, 200), new Vector2(20, 40));

            Assert.AreEqual(1, railGun.Shots.Count);
        }

        [Test]
        public void UsesRayCasterToFindShotEndPoint()
        {
            stubRayCaster.Stub(me => me.ShootRay(Vector2.Zero, Vector2.UnitX)).Return(new Vector2(100, 200));
            stubRayCaster.Stub(me => me.ShootRay(Vector2.UnitX, Vector2.UnitY)).Return(new Vector2(300, 400));

            railGun.Shoot(Vector2.Zero, Vector2.UnitX);
            railGun.Shoot(Vector2.UnitX, Vector2.UnitY);

            Assert.AreEqual(new Vector2(100, 200), railGun.Shots[0].EndPoint);
            Assert.AreEqual(new Vector2(300, 400), railGun.Shots[1].EndPoint);
        }

        [Test]
        public void SetsShotStartPoint()
        {
            railGun.Shoot(new Vector2(10, 20), Vector2.Zero);

            Assert.AreEqual(new Vector2(10, 20), railGun.Shots[0].StartPoint);
        }

    }
}
