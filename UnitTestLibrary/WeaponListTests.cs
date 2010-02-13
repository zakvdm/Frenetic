using System;
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
        List<IPhysicsComponent> hitObjects;
        WeaponList weaponList;
        [SetUp]
        public void SetUp()
        {
            hitObjects = new List<IPhysicsComponent>();
            List<IWeapon> weapons = new List<IWeapon>();
            weapons.Add(MockRepository.GenerateStub<IWeapon>());
            weapons.Add(MockRepository.GenerateStub<IProjectileWeapon>());
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
    }
}