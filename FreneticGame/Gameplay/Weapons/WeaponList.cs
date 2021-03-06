﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Player;
using Frenetic.Physics;

namespace Frenetic.Gameplay.Weapons
{
    public class WeaponList : IWeapons, IEnumerable<IWeapon>
    {
        public WeaponList(Dictionary<WeaponType, IWeapon> weaponList)
        {
            currentWeapon = WeaponType.RailGun;
            this.Shots = new Shots();

            weapons = weaponList;

            foreach (var kvPair in weaponList)
            {
                kvPair.Value.HitAPhysicsComponent += (weap, physicsComp) => this.DamagedAPlayer(weap.Damage, physicsComp);
            }
        }
        #region IWeapons Members

        public IWeapon this[WeaponType type]
        {
            get
            {
                return this.weapons[type];
            }
        }

        public void Shoot(Microsoft.Xna.Framework.Vector2 from, Microsoft.Xna.Framework.Vector2 towards)
        {
            if (from == towards)
                return;

            this[currentWeapon].Shoot(from, towards);
            this.Shots.Add(new Shot(from, towards));
        }

        public void ChangeWeapon(WeaponType weaponType)
        {
            currentWeapon = weaponType;
        }

        public void RemoveDeadProjectiles()
        {
            foreach (var weapon in this)
            {
                weapon.RemoveDeadProjectiles();
            }
        }

        public event Action<int, IPhysicsComponent> DamagedAPlayer = delegate { };

        public Shots Shots
        {
            get;
            private set;
        }

        #endregion

        #region IEnumerable<IWeapon> Members

        public IEnumerator<IWeapon> GetEnumerator()
        {
            foreach (var weapon in this.weapons.Values)
                yield return weapon;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        Dictionary<WeaponType, IWeapon> weapons;
        WeaponType currentWeapon;
    }
}
