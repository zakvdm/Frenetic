﻿using System;
using NUnit.Framework;
using Frenetic.Weapons;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Physics;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class RailGunTests
    {
        IRayCaster stubRayCaster;
        List<IPhysicsComponent> hitObjects;
        RailGun railGun;
        [SetUp]
        public void SetUp()
        {
            hitObjects = new List<IPhysicsComponent>();
            stubRayCaster = MockRepository.GenerateStub<IRayCaster>();
            stubRayCaster.Stub(me => me.ShootRay(Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Equal(Vector2.One), out Arg<Vector2>.Out(Vector2.Zero).Dummy)).Return(hitObjects);
            railGun = new RailGun(stubRayCaster);
        }

        [Test]
        public void CantShootWithDirectionZero()
        {
            railGun.Shoot(Vector2.Zero, Vector2.Zero);

            Assert.AreEqual(0, railGun.Shots.Count);
        }

        [Test]
        public void UsesRayCasterToFindShotEndPoint()
        {
            stubRayCaster.Stub(me => me.ShootRay(Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Equal(Vector2.UnitX), out Arg<Vector2>.Out(new Vector2(100, 200)).Dummy)).Return(new List<IPhysicsComponent>());

            railGun.Shoot(Vector2.Zero, Vector2.UnitX);

            Assert.AreEqual(new Vector2(100, 200), railGun.Shots[0].EndPoint);
        }

        [Test]
        public void SetsShotStartPoint()
        {
            stubRayCaster.Stub(me => me.ShootRay(Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Equal(Vector2.UnitY), out Arg<Vector2>.Out(Vector2.Zero).Dummy)).Return(new List<IPhysicsComponent>());

            railGun.Shoot(new Vector2(10, 20), Vector2.UnitY);

            Assert.AreEqual(new Vector2(10, 20), railGun.Shots[0].StartPoint);
        }

        [Test]
        public void TellsEachHitPhysicsComponentThatItHasBeenHit()
        {
            var physicsComp1 = MockRepository.GenerateStub<IPhysicsComponent>();
            var physicsComp2 = MockRepository.GenerateStub<IPhysicsComponent>();
            hitObjects.Add(physicsComp1);
            hitObjects.Add(physicsComp2);

            railGun.Shoot(Vector2.Zero, Vector2.One);

            physicsComp1.AssertWasCalled(me => me.HitByWeapon());
            physicsComp2.AssertWasCalled(me => me.HitByWeapon());
        }

        [Test]
        public void AddsOffsetToShotOriginSoWeDontShootOurselves()
        {
            stubRayCaster.Stub(me => me.ShootRay(Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Equal(new Vector2(20, 150)), out Arg<Vector2>.Out(Vector2.One).Dummy)).Return(new List<IPhysicsComponent>());
            Vector2 newOrigin = (new Vector2(100, 200)) + (Vector2.Normalize(new Vector2(20, 150) - new Vector2(100, 200)) * RailGun.Offset);
            
            railGun.Shoot(new Vector2(100, 200), new Vector2(20, 150));

            stubRayCaster.AssertWasCalled(me => me.ShootRay(Arg<Vector2>.Is.Equal(newOrigin), Arg<Vector2>.Is.Equal(new Vector2(20, 150)), out Arg<Vector2>.Out(Vector2.One).Dummy));
        }
    }
}