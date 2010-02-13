﻿using System;
using NUnit.Framework;
using Frenetic.Gameplay.Weapons;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Physics;
using System.Collections.Generic;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class WeaponListTests
    {
        WeaponList weaponList;
        [SetUp]
        public void SetUp()
        {
            Dictionary<WeaponType, IWeapon> weapons = new Dictionary<WeaponType, IWeapon>();
            weapons.Add(WeaponType.RailGun, MockRepository.GenerateStub<IWeapon>());
            weapons.Add(WeaponType.RocketLauncher, MockRepository.GenerateStub<IProjectileWeapon>());
            weaponList = new WeaponList(weapons);
        }

        [Test]
        public void ShouldRegistersWithWeaponsForDamagedAPlayerEvent()
        {
            foreach (var stubWeapon in weaponList)
            {
                stubWeapon.AssertWasCalled(weapon => weapon.DamagedAPlayer += Arg<Action<IWeapon, IPhysicsComponent>>.Is.Anything);
            }
        }
        [Test]
        public void ShouldNotifyDamagedPhysicsComponents()
        {
            var stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            IWeapon stubWeapon = null;
            foreach (var weapon in weaponList)
            {
                weapon.Stub(me => me.Damage).Return(12);
                weapon.Raise(me => me.DamagedAPlayer += null, weapon, stubPhysicsComponent);
                break;
            }

            stubPhysicsComponent.AssertWasCalled(me => me.OnWasShot(null, 12));
        }

        [Test]
        public void ShouldCallShootOnTheCurrentWeapon()
        {
            weaponList.ChangeWeapon(WeaponType.RocketLauncher);

            weaponList.Shoot(Vector2.Zero, Vector2.One);

            weaponList[WeaponType.RocketLauncher].AssertWasCalled(me => me.Shoot(Vector2.Zero, Vector2.One));
        }
        [Test]
        public void ShouldNotShootWhenDirectionIsZero()
        {
            weaponList.ChangeWeapon(WeaponType.RailGun);

            weaponList.Shoot(Vector2.Zero, Vector2.Zero);

            weaponList[WeaponType.RailGun].AssertWasNotCalled(me => me.Shoot(Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything));
        }
        [Test]
        public void ShouldCreateShotForEachShoot()
        {
            weaponList.Shoot(Vector2.UnitX, Vector2.UnitY);
            weaponList.Shoot(Vector2.One, new Vector2(2, 2));

            Assert.AreEqual(2, weaponList.Shots.Count);
            Assert.AreEqual(Vector2.UnitX, weaponList.Shots[0].StartPoint);
            Assert.AreEqual(Vector2.UnitY, weaponList.Shots[0].EndPoint);
        }

        [Test]
        public void ShouldCallRemoveDeadProjectilesForAllProjectileWeapons()
        {
            weaponList.RemoveDeadProjectiles();

            foreach (var weapon in weaponList)
            {
                if (weapon is IProjectileWeapon)
                {
                    weapon.AssertWasCalled(weap => (weap as IProjectileWeapon).RemoveDeadProjectiles());
                }
            }
        }
    }
}