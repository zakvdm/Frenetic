using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Gameplay.Weapons
{
    public class WeaponList : IWeapons, IEnumerable<IWeapon>
    {
        public WeaponList(Dictionary<WeaponType, IWeapon> weaponList)
        {
            currentWeapon = WeaponType.RocketLauncher;
            this.Shots = new Shots();

            weapons = weaponList;

            foreach (var kvPair  in weaponList)
            {
                kvPair.Value.DamagedAPlayer += (weap, physicsComp) => physicsComp.OnWasShot(null, weap.Damage);
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
                if (weapon is IProjectileWeapon)
                {
                    (weapon as IProjectileWeapon).RemoveDeadProjectiles();
                }
            }
        }

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
